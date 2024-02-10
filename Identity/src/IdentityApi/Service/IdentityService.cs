using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentValidation;
using IdentityApi.Configurations;
using IdentityApi.Models;
using IdentityApi.Responses;
using IdentityApi.Shared;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using OneOf.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityApi.Service;

public class IdentityService {
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IValidator<TokenDto> _tokenDtoValidator;
    private readonly IValidator<RegistrationUserDto> _registrationUserDtoValidator;
    private readonly IValidator<AuthenticationUserDto> _authUserDtoValidator;
    private readonly JwtConfiguration _jwtConfiguration;

    private User? _user;

    public IdentityService(UserManager<User> userManager, IConfiguration configuration,
        IValidator<TokenDto> tokenDtoValidator,
        IValidator<RegistrationUserDto> registrationUserDtoValidator,
        IValidator<AuthenticationUserDto> authUserDtoValidator) {
        _userManager = userManager;
        _configuration = configuration;
        _tokenDtoValidator = tokenDtoValidator;
        _registrationUserDtoValidator = registrationUserDtoValidator;
        _authUserDtoValidator = authUserDtoValidator;

        _jwtConfiguration = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.Section, _jwtConfiguration);

    }
    public async Task<UserRegisterResponse> RegisterUser(RegistrationUserDto registrationUserDto) {
        if(registrationUserDto is null) {
            return new BadRequestResponse();
        }

        var validationResult = await _registrationUserDtoValidator.ValidateAsync(registrationUserDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            return new ValidationResponse(vaildationFailed);
        }

        List<ValidationError> passwordErrors = new List<ValidationError>();

        var validators = _userManager.PasswordValidators;

        foreach(var validator in validators) {
            var results = await validator.ValidateAsync(_userManager, null, registrationUserDto.Password);

            if(!results.Succeeded) {
                foreach(var error in results.Errors) {
                    passwordErrors.Add(new ValidationError() {
                        PropertyName = nameof(RegistrationUserDto.Password),
                        ErrorMessage = error.Description
                    });
                }
            }
        }

        if(passwordErrors.Count > 0) {
            return new ValidationResponse(passwordErrors);
        }

        var user = registrationUserDto.Adapt<User>();
        var result = await _userManager.CreateAsync(user, registrationUserDto.Password);

        if(result.Succeeded) {
            await _userManager.AddToRolesAsync(user, registrationUserDto.Roles);
        }

        return new Success();
    }
    public async Task<UserValidateResponse> ValidateUser(AuthenticationUserDto authenticationUserDto) {
        if(authenticationUserDto is null) {
            return new BadRequestResponse();
        }

        var validationResult = await _authUserDtoValidator.ValidateAsync(authenticationUserDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            return new ValidationResponse(vaildationFailed);
        }

        _user = await _userManager.FindByNameAsync(authenticationUserDto.UserName);

        if(_user is null) {
            return new UnauthorizedResponse();
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(_user, authenticationUserDto.Password);

        if(!passwordIsValid) {
            return new UnauthorizedResponse();
        }

        var token = await CreateToken(true);

        return token;
    }

    public async Task<TokenRefreshResponse> RefreshToken(TokenDto tokenDto) {
        if(tokenDto is null) {
            return new BadRequestResponse();
        }

        var validationResult = await _tokenDtoValidator.ValidateAsync(tokenDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            return new ValidationResponse(vaildationFailed);
        }

        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if(user is null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) {
            return new RefreshTokenBadRequestResponse();
        }

        _user = user;

        return await CreateToken(populateExp: false);

    }

    private async Task<TokenDto> CreateToken(bool populateExp) {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        _user.RefreshToken = refreshToken;

        if(populateExp)
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }
    private string GenerateRefreshToken() {
        var randomNumber = new byte[32];
        using(var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token) {
        var secret = "";

        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development")) {
            secret = _configuration.GetValue<string>("SECRET");
        }
        else {
            var client = new SecretClient(new Uri(_jwtConfiguration.KeyVaultUri),
                new DefaultAzureCredential(includeInteractiveCredentials: true));
            secret = client.GetSecret(_jwtConfiguration.SecretName).Value.Value;
        }

        var tokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase)) {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials() {
        var secret = "";

        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development")) {
            secret = _configuration.GetValue<string>("SECRET");
        }
        else {
            var client = new SecretClient(new Uri(_jwtConfiguration.KeyVaultUri),
                new DefaultAzureCredential(includeInteractiveCredentials: true));
            secret = client.GetSecret(_jwtConfiguration.SecretName).Value.Value;
        }

        return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims() {
        var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, _user.UserName),
                new Claim(ClaimTypes.NameIdentifier,_user.Id)
            };

        var roles = await _userManager.GetRolesAsync(_user);
        foreach(var role in roles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims) {
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}

[GenerateOneOf]
public partial class TokenRefreshResponse : OneOfBase<TokenDto, ValidationResponse, RefreshTokenBadRequestResponse, BadRequestResponse> {
}
[GenerateOneOf]
public partial class UserValidateResponse : OneOfBase<TokenDto, ValidationResponse, UnauthorizedResponse, BadRequestResponse> {
}
[GenerateOneOf]
public partial class UserRegisterResponse : OneOfBase<Success, ValidationResponse, BadRequestResponse> {
}