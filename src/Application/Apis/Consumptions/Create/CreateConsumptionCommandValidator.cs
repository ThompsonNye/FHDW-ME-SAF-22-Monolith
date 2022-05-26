using System;
using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;

public class CreateConsumptionCommandValidator : AbstractValidator<CreateConsumptionCommand>
{
    private readonly IApplicationDbContext dbContext;

    public CreateConsumptionCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.DateTime.ToUniversalTime())
            .NotNull().WithName(nameof(DateTime)).WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .NotEqual(default(DateTime)).WithName(nameof(DateTime))
            .WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .LessThanOrEqualTo(DateTime.UtcNow).WithName(nameof(DateTime))
            .WithMessage(Resources.ErrorMessageValidationPropertyInFuture);

        RuleFor(x => x.CarId)
            .NotNull().WithMessage(Resources.ErrorMessageValidationPropertyNotProvided)
            .NotEqual(default(Guid)).WithMessage(Resources.ErrorMessageValidationPropertyNotProvided);

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Resources.ErrorMessageValidationPropertyMustBeGreaterOrEqualToZero);

        RuleFor(x => x.Distance)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Resources.ErrorMessageValidationPropertyMustBeGreaterOrEqualToZero);

        this.dbContext = dbContext;
    }
}