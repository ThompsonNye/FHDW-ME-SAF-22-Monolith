using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Common;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _accessor;

    public UserAccessor(IHttpContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    public ClaimsPrincipal User => _accessor.HttpContext.User;

    public Guid UserId => Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

    public string UserName => User.Claims.FirstOrDefault(x => x.Type == Resources.JwtUsernameIdentifier).Value;

    public string FirstName => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;

    public string LastName => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;
}