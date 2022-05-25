using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuyken.VeGasCo.Backend.Application;
using Nuyken.VeGasCo.Backend.Application.Common;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Options;
using Nuyken.VeGasCo.Backend.Infrastructure;
using Nuyken.VeGasCo.Backend.WebApi.Common;
using Nuyken.VeGasCo.Backend.WebApi.Properties;

namespace Nuyken.VeGasCo.Backend.WebApi;

public class Startup
{
    private readonly IConfiguration _configuration;
    private JwtOptions _jwtOptions;
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
        _jwtOptions = _configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddHttpContextAccessor();
        services.AddCors();
        services.AddTransient<IUserAccessor, UserAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddHealthChecks();

        AddAuthentication(services);

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


    private void AddAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = _jwtOptions.Authority;
                options.TokenValidationParameters.ValidAudience = _jwtOptions.Audience;
                options.TokenValidationParameters.ValidIssuer = _jwtOptions.Issuer;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        if (!_jwtOptions.EnableRoleBasedAuthorization) return Task.CompletedTask;

                        var resourceAccessRoles =
                            ctx.Principal?.Claims.FirstOrDefault(x => x.Type == "resource_access");
                        if (resourceAccessRoles is null)
                        {
                            ctx.Fail(new UnauthorizedAccessException(Resources.ErrorMessageUnauthorized));
                            return Task.CompletedTask;
                        }

                        try
                        {
                            var roles = JsonSerializer
                                .Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(
                                    resourceAccessRoles!.Value);

                            if (roles is null || !roles.ContainsKey(_jwtOptions.RoleKey) ||
                                !roles[_jwtOptions.RoleKey]["roles"].Contains(_jwtOptions.RoleValue))
                            {
                                ctx.Fail(new UnauthorizedAccessException(Resources.ErrorMessageUnauthorized));
                                return Task.CompletedTask;
                            }
                        }
                        catch (Exception)
                        {
                            ctx.Fail(Resources.ExceptionMiddlewareUnexpectedError);
                            return Task.CompletedTask;
                        }

                        ctx.Success();
                        return Task.CompletedTask;
                    }
                };
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