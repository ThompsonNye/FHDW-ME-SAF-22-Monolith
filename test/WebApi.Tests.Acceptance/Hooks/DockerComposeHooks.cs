using System.IO;
using System.Linq;
using System.Net;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;
using ConfigurationProvider = Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks.Helpers.ConfigurationProvider;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks;

[Binding]
public sealed class DockerComposeHooks
{
    private static ICompositeService _compositeService;

    [BeforeTestRun]
    public static void DockerComposeUp()
    {
        var dockerComposePath = GetDockerComposePath();
        var apiBasePath = ConfigurationProvider.Configuration.GetValue<string>("ApiBaseAddress");
        _compositeService = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(dockerComposePath)
            .ForceBuild()
            .RemoveOrphans()
            .WaitForHttp("vegasco-dev-app", $"{apiBasePath}/health",
                continuation: (response, _) => response.Code != HttpStatusCode.OK ? 2000 : 0)
            .Build()
            .Start();
    }

    [AfterTestRun]
    public static void DockerComposeDown()
    {
        _compositeService.Stop();
        _compositeService.Dispose();
    }

    private static string GetDockerComposePath()
    {
        var config = ConfigurationProvider.Configuration;
        var composeFileName = config.GetValue<string>("DockerComposeFileName");
        var composeFileExt = Path.GetExtension(composeFileName);

        var directory = Directory.GetCurrentDirectory();

        while (!Directory.EnumerateFiles(directory, $"*{composeFileExt}")
                   .Any(x => x.EndsWith(composeFileName)))
            directory = directory[..directory.LastIndexOf(Path.DirectorySeparatorChar)];

        return Path.Combine(directory, composeFileName);
    }
}