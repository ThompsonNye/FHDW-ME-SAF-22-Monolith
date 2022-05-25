using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

namespace Nuyken.VeGasCo.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();

        return services;
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        if (DbIsMySql(configuration))
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connString = configuration.GetConnectionString(ConfigurationConstants.CONNECTION_STRING_NAME);
                options.UseMySql(connString, ServerVersion.AutoDetect(connString), c =>
                {
                    c.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    c.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                });
            });
        else
            services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("InMemDefault"); });
    }

    private static bool DbIsMySql(IConfiguration configuration)
    {
        var dbType = configuration[ConfigurationConstants.DATABASE_TYPE];
        if (dbType is null) return false;

        var isMySql = new List<string>
        {
            "mysql",
            "mariadb"
        }.Any(x => x.Equals(dbType, StringComparison.InvariantCultureIgnoreCase));

        return isMySql;
    }
}