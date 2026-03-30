using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.Services;

public interface ITokenService
{
    public Result<string, Error> GenerateToken(Guid userId, string email);
}