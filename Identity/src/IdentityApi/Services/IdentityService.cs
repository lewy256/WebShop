using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Contracts.Roles;
using FluentValidation;
using IdentityApi.Configurations;
using IdentityApi.Entities;
using IdentityApi.Responses;
using IdentityApi.Shared;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using OneOf.Types;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IdentityApi.Service;

public class IdentityService {
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegistrationUserDto> _registrationUserDtoValidator;
    private readonly JwtConfiguration _jwtConfiguration;

    private User? _user;

    public IdentityService(UserManager<User> userManager, IConfiguration configuration,
        IValidator<RegistrationUserDto> registrationUserDtoValidator) {
        _userManager = userManager;
        _registrationUserDtoValidator = registrationUserDtoValidator;

        _jwtConfiguration = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.Section, _jwtConfiguration);

    }
    public async Task<UserRegisterResponse> RegisterUser(RegistrationUserDto registrationUserDto) {
        var validationResult = await _registrationUserDtoValidator.ValidateAsync(registrationUserDto);

        var validationErrors = new List<ValidationError>();

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            validationErrors.AddRange(vaildationFailed);
        }

        var user = registrationUserDto.Adapt<User>();

        foreach(var validator in _userManager.UserValidators) {
            var result = await validator.ValidateAsync(_userManager, user);
            if(!result.Succeeded) {
                foreach(var error in result.Errors) {
                    validationErrors.Add(new ValidationError() {
                        PropertyName = Regex.Match(error.Code, "UserName|Email").Value,
                        ErrorMessage = error.Description
                    });
                }
            }
        }

        foreach(var validator in _userManager.PasswordValidators) {
            var result = await validator.ValidateAsync(_userManager, null, registrationUserDto.Password);
            if(!result.Succeeded) {
                foreach(var error in result.Errors) {
                    validationErrors.Add(new ValidationError() {
                        PropertyName = nameof(registrationUserDto.Password),
                        ErrorMessage = error.Description
                    });
                }
            }
        }

        if(validationErrors.Any()) {
            return new ValidationResponse(validationErrors);
        }

        await _userManager.CreateAsync(user, registrationUserDto.Password);
        await _userManager.AddToRolesAsync(user, [UserRole.Customer]);

        return new Success();
    }

    public async Task<UserValidateResponse> ValidateUser(AuthenticationUserDto authenticationUserDto) {
        if(String.IsNullOrEmpty(authenticationUserDto.UserName)
            || String.IsNullOrEmpty(authenticationUserDto.Password)) {
            return new UnauthorizedResponse();
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
        ClaimsPrincipal principal;

        try {
            principal = await GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        }
        catch(Exception ex) {
            Log.Error(ex, "Exception occurred: {Message}", ex.Message);
            return new RefreshTokenBadRequestResponse();
        }


        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if(user is null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) {
            return new RefreshTokenBadRequestResponse();
        }

        _user = user;

        return await CreateToken(populateExp: false);

    }

    private async Task<TokenDto> CreateToken(bool populateExp) {
        var signingCredentials = await GetSigningCredentials();
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

    private async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token) {
        var client = new SecretClient(new Uri(_jwtConfiguration.KeyVaultUri),
            new DefaultAzureCredential());

        var secret = await client.GetSecretAsync(_jwtConfiguration.SecretName);

        var tokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value)),
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

    private async Task<SigningCredentials> GetSigningCredentials() {
        var client = new SecretClient(new Uri(_jwtConfiguration.KeyVaultUri),
            new DefaultAzureCredential());

        var secret = await client.GetSecretAsync(_jwtConfiguration.SecretName);

        return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value)),
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
public partial class TokenRefreshResponse : OneOfBase<TokenDto, RefreshTokenBadRequestResponse> {
}
[GenerateOneOf]
public partial class UserValidateResponse : OneOfBase<TokenDto, UnauthorizedResponse> {
}
[GenerateOneOf]
public partial class UserRegisterResponse : OneOfBase<Success, ValidationResponse> {
}