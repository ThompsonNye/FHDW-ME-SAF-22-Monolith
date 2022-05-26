using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nuyken.VeGasCo.Backend.Domain.Common;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

public class PostgresDbContext : ApplicationDbContext
{
    private readonly IConfiguration _configuration;
    
    public PostgresDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connString = _configuration.GetConnectionString(ConfigurationConstants.CONNECTION_STRING_NAME);
        optionsBuilder.UseNpgsql(connString, b =>
        {
            b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
        });
    }
}