using System;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;

public class UpdateConsumptionCommand : IRequest<Consumption>
{
    public Guid Id { get; set; }

    public double? Amount { get; set; }

    public double? Distance { get; set; }

    public DateTime? DateTime { get; set; }

    public bool? IgnoreInCalculation { get; set; }

    public Guid? CarId { get; set; }

    /// <summary>
    ///     Updates the given <paramref name="instance" /> with the values in this object.
    /// </summary>
    /// <param name="instance"></param>
    public void Update(ref Consumption instance)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        // Id omitted because it has to be the same since the instance will be found based in this.Id
        // UserId omitted because consumption entry cannot be transferred to other users

        instance.Amount = Amount ?? instance.Amount;
        instance.Distance = Distance ?? instance.Distance;
        instance.DateTime = DateTime ?? instance.DateTime;
        instance.CarId = CarId ?? instance.CarId;
        instance.IgnoreInCalculation = IgnoreInCalculation ?? instance.IgnoreInCalculation;
    }
}