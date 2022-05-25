using System;
using MediatR;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;

public class DeleteCarCommand : IRequest
{
    public Guid Id { get; set; }
}