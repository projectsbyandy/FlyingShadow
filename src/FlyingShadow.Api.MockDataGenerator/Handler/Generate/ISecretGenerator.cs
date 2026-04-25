namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate;

public interface ISecretGenerator
{
    string Jwt();
    string Password(int length = 16);
}