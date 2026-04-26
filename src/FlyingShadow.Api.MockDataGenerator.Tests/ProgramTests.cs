using FlyingShadow.Api.MockDataGenerator.Ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FlyingShadow.Api.MockDataGenerator.Tests;

public class ProgramTests : IDisposable
{
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token;
    private IHost? _host;
    
    [Theory]
    [InlineData([new[] {"FakeJwtPath","FakeLoginDetailsListPath","FakeUsersPath","FakeShadowsPath", "FakeStealthMetricsPath"}])]
    [InlineData([new[] {"FakeJwtPath","FakeLoginDetailsListPath","FakeUsersPath","FakeShadowsPath"}])]
    [InlineData([new[] {"FakeJwtPath","FakeLoginDetailsListPath","FakeUsersPath"}])]
    [InlineData([new[] {"FakeJwtPath","FakeLoginDetailsListPath"}])]
    [InlineData([new[] {"FakeJwtPath"}])]
    public async Task Program_With_MissingArguments_ThrowsException(string[] missingFields)
    {
        // Arrange
        var missingFieldsList = missingFields.ToList();
        
        var configValues = new Dictionary<string, string?>()
        {
            ["FakeJwtPath"] = "present",
            ["FakeLoginDetailsListPath"] = "present",
            ["FakeUsersPath"] = "present",
            ["FakeShadowsPath"] = "present",
            ["FakeStealthMetricsPath"] = "present",
        };
            
        missingFieldsList.ForEach(missingField =>
        {
            if (configValues.ContainsKey(missingField))
                configValues[missingField] = string.Empty;
        });
        
        var configOverride = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues).Build();
        
        var builder = Host.CreateApplicationBuilder();
        builder.Services
            .AddMockDataGenerator()
            .ProcessArguments(configOverride);
        
        _host = builder.Build();
        
        // Act
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(
            () => _host.StartAsync(_cancellationToken));
        
        // Assert
        Assert.Equal(missingFieldsList.Count, exception.Failures.Count());
        missingFieldsList.ForEach(missingField => 
            Assert.Contains(exception.Failures, 
                f => f.Contains(missingField)));
    }

    public void Dispose()
    { 
        _host?.StopAsync(_cancellationToken);
    }
}