using System;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;

public class UpdateCarCommand : IRequest<Car>
{
    public Guid Id { get; set; }

    public string Name { get; set; }


    /// <summary>
    ///     Updates the given <paramref name="instance" /> with the values in this object.
    /// </summary>
    /// <param name="instance"></param>
    public void Update(ref Car instance)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        // Id omitted because it has to be the same since the instance will be found based in this.Id
        // UserId omitted because consumption entry cannot be transferred to other users

        instance.Name = Name ?? instance.Name;
    }
}