using System;
using System.Linq;
using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;

public class CreateConsumptionCommandValidator : AbstractValidator<CreateConsumptionCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public CreateConsumptionCommandValidator(IUserAccessor userAccessor, IApplicationDbContext dbContext)
    {
        RuleFor(x => x.DateTime.ToUniversalTime())
            .NotNull().WithName(nameof(DateTime)).WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .NotEqual(default(DateTime)).WithName(nameof(DateTime))
            .WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .LessThanOrEqualTo(DateTime.UtcNow).WithName(nameof(DateTime))
            .WithMessage(Resources.ErrorMessageValidationPropertyInFuture);

        RuleFor(x => x.CarId)
            .NotNull().WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .Must(BeOwnedByUser).WithMessage(Resources.ErrorMessageValidationPropertyNotFoundForUser)
            .NotEqual(default(Guid)).WithMessage(Resources.ErrorMessageValidationPropertyNotProvided);

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Resources.ErrorMessageValidationPropertyMustBeGreaterOrEqualToZero);

        RuleFor(x => x.Distance)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Resources.ErrorMessageValidationPropertyMustBeGreaterOrEqualToZero);
        this.userAccessor = userAccessor;
        this.dbContext = dbContext;
    }

    protected bool BeOwnedByUser(Guid carId)
    {
        var car = dbContext.Cars.FirstOrDefault(x => x.Id == carId);
        var userId = userAccessor.UserId;
        return car is not null && car.UserId == userId;
    }
}