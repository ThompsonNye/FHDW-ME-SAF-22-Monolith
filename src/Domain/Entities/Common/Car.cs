using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nuyken.VeGasCo.Backend.Domain.Entities.Common;

public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public virtual List<Consumption> Consumptions { get; set; }
}