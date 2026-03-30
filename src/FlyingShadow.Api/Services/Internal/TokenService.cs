using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Models.ResultType;
using Microsoft.IdentityModel.Tokens;

namespace FlyingShadow.Api.Services.Internal;

internal class TokenService : ITokenService
{
    private readonly Configuration _configuration;

    public TokenService(Configuration configuration)
    {
        _configuration = configuration;
    }

    public Result<string, Error> GenerateToken(Guid userId, string email)
    {
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.Jwt!.Key!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration.Jwt.Issuer,
                audience: _configuration.Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Result<string, Error>.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }
        catch (Exception ex)
        {
            return Result<string, Error>.Failure(new Error("TOKEN_GENERATION_ERROR", ex.Message));
        }
    }
}