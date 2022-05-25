using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Nuyken.VeGasCo.Backend.Domain.Common;

namespace Nuyken.VeGasCo.Backend.Application.Common.Extensions;

public static class ConfigurationValueProcessor
{
    private static readonly Dictionary<string, string> keyMappings = new()
    {
        {
            ConfigurationConstants.DEFAULT_CONNECTION_STRING_ENV_NAME,
            ConfigurationConstants.DEFAULT_CONNECTION_STRING
        }
    };

    public static IConfiguration ProcessConfigurationValues(this IConfiguration configuration)
    {
        foreach (var mapping in keyMappings)
            if (configuration[mapping.Key] is string value && !string.IsNullOrEmpty(value))
                configuration[mapping.Value] = value;

        return configuration;
    }
}