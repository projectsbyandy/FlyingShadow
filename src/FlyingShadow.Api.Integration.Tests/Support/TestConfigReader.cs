using FlyingShadow.Api.Utils;
using Microsoft.Extensions.Configuration;

namespace FlyingShadow.Api.Integration.Tests.Support;

internal static class TestConfigReader
{
    public static void Add(string jsonFileName)
    { 
        var testConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(jsonFileName, optional: false)
            .Build();

        var merged = new ConfigurationBuilder()
            .AddConfiguration(ConfigReader.GetCurrentConfiguration()) 
            .AddConfiguration(testConfig)                              
            .Build();
        
        ConfigReader.SetConfiguration(merged);
    }

    public static void Reset() => ConfigReader.SetConfiguration(null);
}