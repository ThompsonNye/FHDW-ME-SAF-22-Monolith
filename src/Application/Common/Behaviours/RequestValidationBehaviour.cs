﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ValidationException = Nuyken.VeGasCo.Backend.Domain.Common.Exceptions.ValidationException;

namespace Nuyken.VeGasCo.Backend.Application.Common.Behaviours;

public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0) throw new ValidationException(failures);

        return next();
    }
}