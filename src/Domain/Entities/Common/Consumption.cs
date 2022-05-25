using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nuyken.VeGasCo.Backend.Domain.Entities.Common;

public class Consumption
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime DateTime { get; set; }

    public double Distance { get; set; }

    public double Amount { get; set; }

    public bool IgnoreInCalculation { get; set; }


    [ForeignKey(nameof(Car))] public Guid CarId { get; set; }

    [JsonIgnore] public virtual Car Car { get; set; }

    public virtual string CarName => Car?.Name;
}