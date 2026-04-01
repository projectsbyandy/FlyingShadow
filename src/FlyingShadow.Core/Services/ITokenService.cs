using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface ITokenService
{
    public Result<string, Error> GenerateToken(Guid userId, string email);
}