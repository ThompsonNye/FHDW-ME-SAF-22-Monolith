using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Nuyken.VeGasCo.Backend.Domain.Properties;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException() : base(Resources.ValidationExceptionMessage)
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(List<ValidationFailure> failures) : this()
    {
        var failureGroups = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

        foreach (var failureGroup in failureGroups)
        {
            var propertyName = failureGroup.Key;
            var propertyFailures = failureGroup.ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }

    public IDictionary<string, string[]> Failures { get; }
}