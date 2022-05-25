using Microsoft.Extensions.Configuration;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks.Helpers;

public static class ConfigurationProvider
{
    private static IConfiguration _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            return _configuration ??= new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<IAssemblyMarker>()
                .Build();
        }
    }
}