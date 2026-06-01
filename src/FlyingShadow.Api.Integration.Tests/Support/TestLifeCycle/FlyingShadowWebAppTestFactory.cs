using FlyingShadow.Api.Ioc;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;

public class FlyingShadowWebAppTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            // Registering as scoped so JWT expiry can be updated within test
            services.RemoveAll<Configuration>();
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IShadowRepository>();
            services.RemoveAll<IStealthMetricsRepository>();
            
            services.AddScoped<Configuration>(_ => ConfigReader.GetConfiguration<Configuration>());
            services.AddRepositories(isMock: true);
        });
    }
}