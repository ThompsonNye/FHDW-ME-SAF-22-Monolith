using System;
using System.Security.Claims;

namespace Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;

public interface IUserAccessor
{
    ClaimsPrincipal User { get; }

    Guid UserId { get; }

    string UserName { get; }

    string FirstName { get; }

    string LastName { get; }
}