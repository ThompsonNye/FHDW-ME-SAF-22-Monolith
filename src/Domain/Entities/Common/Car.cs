using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nuyken.VeGasCo.Backend.Domain.Entities.Common;

public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }


    public virtual List<Consumption> Consumptions { get; set; }


    public Guid UserId { get; set; }

    [JsonIgnore] public virtual User User { get; set; }
}