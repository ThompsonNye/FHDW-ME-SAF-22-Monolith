using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Delete;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Get;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.WebApi.Controllers;

public class ConsumptionsController : BaseController
{
    /// <summary>
    ///     Returns consumption entries for the user. By default only returns the entries which have not been deleted (see
    ///     <paramref name="all" />).
    /// </summary>
    /// <param name="all">Whether to also return deleted entries (true) or only non-deleted entries (false).</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Consumption>>> GetAsync([FromQuery] bool all)
    {
        GetConsumptionsQuery query = new();

        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    ///     Creates a consumption entry.
    /// </summary>
    /// <param name="command">The consumption details.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateConsumptionCommand command)
    {
        var consumption = await Mediator.Send(command);

        return Created($"/api/{nameof(Consumption)}s/{consumption.Id}", consumption);
    }

    /// <summary>
    ///     Update a consumption entry. Only updates the details provided and keeps the remaining details.
    /// </summary>
    /// <param name="command">The consumption details to update.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Consumption>> PutAsync([FromBody] UpdateConsumptionCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Deletes a consumption entry.
    /// </summary>
    /// <param name="id">The entry's id.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var command = new DeleteConsumptionCommand {Id = id};

        await Mediator.Send(command);

        return NoContent();
    }
}