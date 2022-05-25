using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;

public class UploadSettingsCommandValidator : AbstractValidator<UploadSettingsCommand>
{
    public UploadSettingsCommandValidator()
    {
        RuleFor(x => x)
            .Must(HaveAnyData).WithMessage(Resources.ErrorMessageValidationAnyDataNeeded);

        RuleFor(x => x.AppColorDesign)
            .Must(BeValidAppColorDesign).WithMessage(Resources.ErrorMessageValidationPropertyInvalidValue);
    }


    private bool HaveAnyData(UploadSettingsCommand command)
    {
        var checks = new List<bool>
        {
            command.AppColorDesign != null
        };
        return checks.Any(x => x);
    }

    protected bool BeValidAppColorDesign(UploadSettingsCommand command, AppColorDesign? value)
    {
        if (!value.HasValue) return HaveAnyData(command);

        var success = Enum.IsDefined(value.Value);

        return success;
    }
}