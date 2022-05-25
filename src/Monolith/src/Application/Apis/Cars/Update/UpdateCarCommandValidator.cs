using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Properties;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;

public class UpdateCarCommandValidator : AbstractValidator<UpdateCarCommand>
{
    private readonly IActionContextAccessor actionContextAccessor;

    public UpdateCarCommandValidator(IActionContextAccessor actionContextAccessor)
    {
        this.actionContextAccessor =
            actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));

        RuleFor(x => x)
            .Must(HaveNonDefaultRouteId).WithName("RouteId")
            .WithMessage(Resources.ErrorMessageValidationRouteIdEmptyOrDefaultGuid);

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(MatchRouteValue).WithMessage(Resources.ErrorMessageValidationPropertyBodyRouteMismatch);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(BeValidCharacters).WithMessage(Resources.ErrorMessageValidationPropertyContainsInvalidCharacters);
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

    protected bool HaveNonDefaultRouteId(UpdateCarCommand _)
    {
        var firstIdRouteValue = actionContextAccessor.ActionContext.RouteData.Values
            .Where(x => x.Key == "id")
            .FirstOrDefault();

        var routeIdString = firstIdRouteValue.Value as string;

        if (string.IsNullOrEmpty(routeIdString)) return false;

        var canBeParsed = Guid.TryParse(routeIdString, out var parsedId);

        return canBeParsed && parsedId != default;
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