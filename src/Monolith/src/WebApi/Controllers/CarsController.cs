using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.WebApi.Controllers;

public class CarsController : BaseController
{
    /// <summary>
    ///     Returns car entries for the user. By default only returns the entries which have not been deleted (see
    ///     <paramref name="all" />).
    /// </summary>
    /// <param name="all">Whether to also return deleted entries (true) or only non-deleted entries (false).</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Car>>> GetAllAsync([FromQuery] bool all)
    {
        GetCarsQuery query = new();

        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    ///     Creates a new car entry.
    /// </summary>
    /// <param name="command">The car details.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Car>> PostAsync([FromBody] CreateCarCommand command)
    {
        var car = await Mediator.Send(command);

        return Created($"/api/{nameof(Car)}s/{car.Id}", car);
    }

    /// <summary>
    ///     Updates a car entry. Only updates the details provided and keeps the remaining details.
    /// </summary>
    /// <param name="command">The car details.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Car>> PutAsync([FromBody] UpdateCarCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Deletes a car entry.
    /// </summary>
    /// <param name="id">The entry's id.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var command = new DeleteCarCommand {Id = id};

        await Mediator.Send(command);

        return NoContent();
    }
}