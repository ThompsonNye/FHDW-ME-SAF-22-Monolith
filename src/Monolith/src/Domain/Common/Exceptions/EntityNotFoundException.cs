using System;
using Nuyken.VeGasCo.Backend.Domain.Properties;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, string entityValue) : base(
        string.Format(Resources.EntityNotFoundExceptionMessageTemplate, entityName, entityValue))
    {
        EntityName = entityName;
        EntityValue = entityValue;
    }

    public string EntityName { get; }
    public string EntityValue { get; }
}