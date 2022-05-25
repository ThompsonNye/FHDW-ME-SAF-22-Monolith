using System;
using FluentValidation;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;

public class DeleteCarCommandValidator : AbstractValidator<DeleteCarCommand>
{
    public DeleteCarCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(default(Guid));
    }
}