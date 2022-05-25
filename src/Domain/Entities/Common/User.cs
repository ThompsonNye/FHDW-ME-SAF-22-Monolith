using System;
using System.Collections.Generic;

namespace Nuyken.VeGasCo.Backend.Domain.Entities.Common;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }


    public virtual List<Car> Cars { get; set; }

    public virtual List<Consumption> Consumptions { get; set; }

    public virtual List<Setting> Settings { get; set; }
}