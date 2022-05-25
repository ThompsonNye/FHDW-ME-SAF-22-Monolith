using System;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

public class MigrationImpossibleException : Exception
{
    public MigrationImpossibleException()
    {
    }

    public MigrationImpossibleException(string message) : base(message)
    {
    }

    public MigrationImpossibleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}