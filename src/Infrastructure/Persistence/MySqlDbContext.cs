using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nuyken.VeGasCo.Backend.Domain.Common;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

public class MySqlDbContext : ApplicationDbContext
{
    private readonly IConfiguration _configuration;

    public MySqlDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connString = _configuration.GetConnectionString(ConfigurationConstants.CONNECTION_STRING_NAME);
        optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString), c =>
        {
            c.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            c.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
        });
    }
}