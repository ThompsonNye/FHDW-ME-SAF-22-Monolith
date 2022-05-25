using System;
using System.Text.Json.Serialization;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;

namespace Nuyken.VeGasCo.Backend.Domain.Entities.Common;

public class Setting
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public AppColorDesign? AppColorDesign { get; set; }


    public Guid UserId { get; set; }

    [JsonIgnore] public virtual User User { get; set; }
}