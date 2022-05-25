using System;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;
using Nuyken.VeGasCo.Backend.Domain.Properties;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

public class InvalidOrMissingConfigurationException<T> : ApplicationException
{
    public InvalidOrMissingConfigurationException(string name, T value, InvalidOrMissingConfigurationReason reason)
        : base(MakeMessage(name, value, reason))
    {
        Name = name;
        Value = value;
    }

    public InvalidOrMissingConfigurationException(string name, T value, InvalidOrMissingConfigurationReason reason,
        Exception innerException)
        : base(MakeMessage(name, value, reason), innerException)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    ///     The name of the configuration option
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The value of the configuration option
    /// </summary>
    public T Value { get; set; }

    private static string MakeMessage(string name, T value, InvalidOrMissingConfigurationReason reason)
    {
        return string.Format(Resources.InvalidOrMissingConfigurationExceptionMessageTemplate, name,
            reason.ToString().ToLower(), value?.ToString() ?? "null");
    }
}