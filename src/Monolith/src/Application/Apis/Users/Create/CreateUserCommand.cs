using System;
using MediatR;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Create;

public class CreateUserCommand : IRequest
{
    public Guid Id { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}