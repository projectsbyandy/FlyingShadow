using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.DTO.Token;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services;
using Microsoft.IdentityModel.Tokens;

namespace FlyingShadow.Api.Services;

internal class TokenService : ITokenService
{
    private readonly Configuration _configuration;

    public TokenService(Configuration configuration)
    {
        _configuration = configuration;
    }

    public Result<TokenDetails, Error> GenerateToken(Guid userId, string email)
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
                expires: DateTime.UtcNow.AddHours(_configuration.Jwt.ExpiryInHours),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Result<TokenDetails, Error>.Success(new TokenDetails(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo));
        }
        catch (Exception ex)
        {
            return Result<TokenDetails, Error>.Failure(new Error(ErrorCode.UnexpectedError, ex.Message));
        }
    }
}