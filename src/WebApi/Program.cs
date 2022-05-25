using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nuyken.VeGasCo.Backend.Application;
using Nuyken.VeGasCo.Backend.Application.Common;
using Nuyken.VeGasCo.Backend.Application.Common.Extensions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.WebApi.Properties;
using Serilog;

namespace Nuyken.VeGasCo.Backend.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();

            var configuration = (host.Services.GetService(typeof(IConfiguration)) as IConfiguration)
                .ProcessConfigurationValues();
            new StartupChecks(configuration).EnsureConfigurationValues();
            await new InitializeManager(host).InitializeAsync();

            host.Run();
        }
        catch (InvalidOrMissingConfigurationException<object> ex)
        {
            EnsureLogger();

            Log.Fatal(ex, Resources.ErrorMessageInvalidOrMissingConfigurationOption);
        }
        catch (Exception ex)
        {
            EnsureLogger();

            Log.Fatal(ex, Resources.ErrorMessageUnexpectedHostTermination);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .CaptureStartupErrors(true);
            });
    }

    /// <summary>
    ///     Ensures a logger is configured.
    /// </summary>
    private static void EnsureLogger()
    {
        // Log.Logger will likely be internal type "Serilog.Core.Pipeline.SilentLogger".
        if (Log.Logger.GetType().Name == "SilentLogger")
            // Loading configuration or Serilog failed.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
    }
}