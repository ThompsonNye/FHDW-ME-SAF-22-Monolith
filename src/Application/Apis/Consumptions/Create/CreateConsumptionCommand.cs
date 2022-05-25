using System;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;

public class CreateConsumptionCommand : IRequest<Consumption>
{
    public Guid? Id { get; set; }

    public DateTime DateTime { get; set; }

    public double Distance { get; set; }

    public double Amount { get; set; }

    public bool IgnoreInCalculation { get; set; }

    public Guid CarId { get; set; }
}