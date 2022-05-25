using System;
using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Properties;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Delete;

public class DeleteConsumptionCommandValidator : AbstractValidator<DeleteConsumptionCommand>
{
    public DeleteConsumptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(default(Guid)).WithMessage(Resources.ErrorMessageValidationPropertyNotProvided);
    }
}