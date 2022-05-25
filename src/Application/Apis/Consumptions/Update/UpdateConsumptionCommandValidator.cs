using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;

public class UpdateConsumptionCommandValidator : AbstractValidator<UpdateConsumptionCommand>
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public UpdateConsumptionCommandValidator(IActionContextAccessor actionContextAccessor,
        IApplicationDbContext dbContext, IUserAccessor userAccessor)
    {
        this.actionContextAccessor =
            actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));

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

        RuleFor(x => x.CarId)
            .Must(BeOwnedByUserIfProvided).WithMessage("{PropertyName} cannot be found for the user");
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

    protected bool BeOwnedByUserIfProvided(Guid? carId)
    {
        if (carId == default) return true;

        var car = dbContext.Cars.FirstOrDefault(x => x.Id == carId);
        var userId = userAccessor.UserId;
        return car is not null && car.UserId == userId;
    }
}