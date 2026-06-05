using FlyingShadow.Core.DTO.Configuration;

namespace FlyingShadow.Core.Utils;

public class DbConnectionStringBuilder
{
    public static string Map(DbServer dbConfig)
    {
        return
            $"Host={dbConfig.Host};Port={dbConfig.Port};Database={dbConfig.Database};" +
            $"Username={dbConfig.Username};Password={dbConfig.Password};" +
            $"Maximum Pool Size={dbConfig.MaxPoolSize};Minimum Pool Size={dbConfig.MinPoolSize}";
    }
}