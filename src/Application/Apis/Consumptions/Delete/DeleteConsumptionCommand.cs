using System;
using MediatR;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Delete;

public class DeleteConsumptionCommand : IRequest
{
    public Guid Id { get; set; }
}