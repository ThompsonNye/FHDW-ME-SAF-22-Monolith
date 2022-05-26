using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Nuyken.VeGasCo.Backend.Domain.Common;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Common.Options;

namespace Nuyken.VeGasCo.Backend.Application.Common;

public class StartupChecks
{
    private readonly IConfiguration configuration;

    public StartupChecks(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void EnsureConfigurationValues()
    {
        var databaseType = configuration[ConfigurationConstants.DATABASE_TYPE];
        EnsureDatabaseType(databaseType);

        EnsureConnectionString(configuration[ConfigurationConstants.DEFAULT_CONNECTION_STRING], databaseType);
    }

    /// <summary>
    ///     Ensures valid values for the <paramref name="databaseType" /> if it is not null.
    /// </summary>
    /// <param name="databaseType"></param>
    private static void EnsureDatabaseType(string databaseType)
    {
        // Value not required
        if (string.IsNullOrEmpty(databaseType)) return;

        // Only certain values allowed
        var allowed = new[]
        {
            "mariadb",
            "mysql",
            "postgres",
            "postgresql"
        };
        if (!allowed.Any(x => x.Equals(databaseType, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOrMissingConfigurationException<string>(
                "Database type",
                databaseType,
                InvalidOrMissingConfigurationReason.Invalid);
        }
    }

    /// <summary>
    ///     Ensures the <paramref name="connectionString" /> is not null if the <paramref name="databaseType" /> is not null.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="databaseType"></param>
    private static void EnsureConnectionString(string connectionString, string databaseType)
    {
        // Connection string is only required if the database type is not null.
        // The validity of the database type is checked before this.
        if (string.IsNullOrEmpty(databaseType)) return;

        // Required since database type is not null
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOrMissingConfigurationException<string>("Connection string", connectionString,
                InvalidOrMissingConfigurationReason.Missing);
    }
}