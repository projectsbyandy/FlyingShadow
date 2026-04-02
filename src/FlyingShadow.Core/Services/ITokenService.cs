using FlyingShadow.Core.DTO.Token;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface ITokenService
{
    public Result<TokenDetails, Error> GenerateToken(Guid userId, string email);
}