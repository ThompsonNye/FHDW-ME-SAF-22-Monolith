using System;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

public class DuplicateEntryException : ApplicationException
{
    public DuplicateEntryException() { }

    public DuplicateEntryException(string message) : base(message) { }
}