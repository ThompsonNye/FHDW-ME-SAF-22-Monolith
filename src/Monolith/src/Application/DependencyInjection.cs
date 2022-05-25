using System.Diagnostics;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuyken.VeGasCo.Backend.Application.Common.Behaviours;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Nuyken.VeGasCo.Backend.Application;

public static class DependencyInjection
{
    /// <summary>
    ///     Add <see cref="Application" /> specific services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestNotNullBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));

        return services;
    }

    public static void ConfigureLogging(this LoggerConfiguration loggerConfiguration,
        WebHostBuilderContext hostingContext)
    {
        if (hostingContext.HostingEnvironment.IsDevelopment())
            loggerConfiguration.ConfigureDevelopmentLogging();
        else
            loggerConfiguration.ConfigureProductionLogging();

        loggerConfiguration
            .WriteTo.Console(LogEventLevel.Information,
                "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", Assembly.GetExecutingAssembly().GetName().Name)
            .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);

#if DEBUG
        // Used to filter out potentially bad data due debugging.
        // Very useful when doing Seq dashboards and want to remove logs under debugging session.
        loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
    }

    private static void ConfigureDevelopmentLogging(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration
            .MinimumLevel.Debug();
    }


    private static void ConfigureProductionLogging(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
    }
}