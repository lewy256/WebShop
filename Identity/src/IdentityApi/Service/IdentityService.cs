using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using IdentityApi.Exceptions;
using IdentityApi.Models;
using IdentityApi.Models.Shared;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityApi.Service;

public class IdentityService {
    private readonly UserManager<User> _userManager;
    private readonly JwtConfiguration _jwtConfiguration;

    private User? _user;

    public IdentityService(UserManager<User> userManager, IConfiguration configuration) {
        _userManager = userManager;

        _jwtConfiguration = new JwtConfiguration();
        configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);

    }


    public async Task<IdentityResult> RegisterUser(RegistrationUserDto registrationUserDto) {
        var user = registrationUserDto.Adapt<User>();

        var result = await _userManager.CreateAsync(user, registrationUserDto.Password);
        if(result.Succeeded)
            await _userManager.AddToRolesAsync(user, registrationUserDto.Roles);
        return result;
    }
    public async Task<bool> ValidateUser(AuthenticationUserDto authenticationUserDto) {
        _user = await _userManager.FindByNameAsync(authenticationUserDto.UserName);

        var result = (_user != null && await _userManager.CheckPasswordAsync(_user, authenticationUserDto.Password));
        if(!result)
            Log.Warning($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");

        return result;
    }

    public async Task<TokenDto> CreateToken(bool populateExp) {
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

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto) {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if(user == null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();

        _user = user;

        return await CreateToken(populateExp: false);
    }

    private string GenerateRefreshToken() {
        var randomNumber = new byte[32];
        using(var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token) {
        var secretName = _jwtConfiguration.SecretName;
        var keyVaultUri = _jwtConfiguration.KeyVaultUri;

        var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(includeInteractiveCredentials: true));
        var secretResponse = client.GetSecret(secretName);

        var tokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretResponse.Value.Value)),
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
        var secretName = _jwtConfiguration.SecretName;
        var keyVaultUri = _jwtConfiguration.KeyVaultUri;

        var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(includeInteractiveCredentials: true));
        var secretResponse = client.GetSecret(secretName);

        var key = Encoding.UTF8.GetBytes(secretResponse.Value.Value);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims() {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

        var roles = await _userManager.GetRolesAsync(_user);
        foreach(var role in roles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims) {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}
