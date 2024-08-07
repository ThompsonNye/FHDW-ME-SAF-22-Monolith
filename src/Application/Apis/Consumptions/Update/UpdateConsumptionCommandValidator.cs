﻿using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;

public class UpdateConsumptionCommandValidator : AbstractValidator<UpdateConsumptionCommand>
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IApplicationDbContext dbContext;

    public UpdateConsumptionCommandValidator(IActionContextAccessor actionContextAccessor,
        IApplicationDbContext dbContext)
    {
        this.actionContextAccessor =
            actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEqual(default(Guid)).WithMessage("{PropertyName} not provided")
            .Must(MatchRouteValue).WithMessage("{PropertyName} in route does not match {PropertyName} in body");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Distance)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DateTime)
            .Must(NotBeInFutureIfProvided).WithName(nameof(DateTime))
            .WithMessage("{PropertyName} cannot be in the future");
    }

    protected bool MatchRouteValue(Guid id)
    {
        var firstIdRouteValue = actionContextAccessor.ActionContext.RouteData.Values
            .Where(x => x.Key == "id")
            .FirstOrDefault();

        var routeId = firstIdRouteValue.Value as string;

        var idsMatch = routeId is not null &&
                       id.ToString().Equals(routeId, StringComparison.InvariantCultureIgnoreCase);

        return idsMatch;
    }

    protected static bool NotBeInFutureIfProvided(DateTime? dateTime)
    {
        if (!dateTime.HasValue) return true;

        return dateTime.Value.ToUniversalTime() <= DateTime.UtcNow;
    }
}