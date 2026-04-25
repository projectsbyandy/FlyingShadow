using System.Security.Cryptography;
using System.Text;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;

internal class SecretGenerator : ISecretGenerator
{
    public string Jwt() => Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

    public string Password(int length = 16)
    {
        const string chars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$";
        var maxValid = 256 - (256 % chars.Length);
        var sb = new StringBuilder(length);

        while (sb.Length < length)
        {
            foreach (var b in RandomNumberGenerator.GetBytes(length * 2))
            {
                if (b >= maxValid) continue;
                sb.Append(chars[b % chars.Length]);
                if (sb.Length == length) break;
            }
        }

        return sb.ToString();
    }
}