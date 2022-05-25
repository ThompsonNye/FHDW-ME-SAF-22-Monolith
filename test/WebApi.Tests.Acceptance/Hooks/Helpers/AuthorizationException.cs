using System;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks.Helpers;

public class AuthorizationException : ApplicationException
{
    public AuthorizationException()
    {
    }

    public AuthorizationException(string message) : base(message)
    {
    }
}