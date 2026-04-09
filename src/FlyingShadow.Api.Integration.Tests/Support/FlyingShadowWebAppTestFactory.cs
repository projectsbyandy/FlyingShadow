using FlyingShadow.Api.Ioc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyingShadow.Api.Integration.Tests.Support;

public class FlyingShadowWebAppTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            services.RegisterFakeJsonRepositories();
        });
    }
}