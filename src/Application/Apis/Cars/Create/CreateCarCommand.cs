using System;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;

public class CreateCarCommand : IRequest<Car>
{
    public Guid? Id { get; set; }

    public string Name { get; set; }
}