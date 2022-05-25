using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuyken.VeGasCo.Backend.Application;
using Nuyken.VeGasCo.Backend.Application.Common;
using Nuyken.VeGasCo.Backend.Domain.Common.Options;
using Nuyken.VeGasCo.Backend.Infrastructure;
using Nuyken.VeGasCo.Backend.WebApi.Common;

namespace Nuyken.VeGasCo.Backend.WebApi;

public class Startup
{
    private readonly IConfiguration _configuration;
    private SwaggerOptions _swaggerOptions;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure(_configuration);

        services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<StartupChecks>())
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.Configure<DatabaseOptions>(_configuration.GetSection("Database"));
        _swaggerOptions = _configuration.GetSection("Swagger").Get<SwaggerOptions>();

        services.AddHttpContextAccessor();
        services.AddCors();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddHealthChecks();

        AddSwagger(services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        UseSwagger(app, env.IsDevelopment() || _swaggerOptions.PublishSwaggerUI);

        app.UseCors(cors =>
        {
            cors.AllowAnyOrigin();
            cors.AllowAnyMethod();
            cors.AllowAnyHeader();
        });

        app.UseCustomExceptionHandler();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }

    private void AddSwagger(IServiceCollection services)
    {
        services.AddOpenApiDocument(doc =>
        {
            doc.Title = _swaggerOptions.Title;
            doc.Description = _swaggerOptions.Description;
            doc.Version = ThisAssembly.AssemblyVersion;

            doc.GenerateEnumMappingDescription = true;
        });
    }

    private static void UseSwagger(IApplicationBuilder app, bool publishUi = false)
    {
        app.UseOpenApi();
        if (publishUi)
            app.UseSwaggerUi3(c => { c.EnableTryItOut = false; });
    }
}