using System;
using System.Text.RegularExpressions;
using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Properties;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;

public class CreateCarCommandValidator : AbstractValidator<CreateCarCommand>
{
    public CreateCarCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(BeValidCharacters).WithMessage(Resources.ErrorMessageValidationPropertyContainsInvalidCharacters);
    }

    protected static bool BeValidCharacters(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException($"'{nameof(s)}' cannot be null or empty.", nameof(s));

        var regex = new Regex(Resources.ValidationRegexValidCharacters);
        var isMatch = regex.IsMatch(s);
        return isMatch;
    }
}