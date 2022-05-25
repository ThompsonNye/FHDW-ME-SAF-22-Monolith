using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nuyken.VeGasCo.Backend.Application.Apis.Info.User;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Delete;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Get;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.WebApi.Controllers;

public class UserController : BaseController
{
    /// <summary>
    ///     Returns information about the calling user retrieved from the provided JWT.
    /// </summary>
    /// <returns></returns>
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserInfoResponse))]
    public async Task<ActionResult<GetUserInfoResponse>> GetUserInfoAsync()
    {
        var query = new GetUserInfoQuery();
        return await Mediator.Send(query);
    }

    /// <summary>
    ///     <b>Completely</b> deletes the user and all associated entries.
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserAsync()
    {
        await Mediator.Send(new DeleteUserCommand());

        return NoContent();
    }

    /// <summary>
    ///     Returns the settings for the calling user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Setting))]
    public async Task<ActionResult<Setting>> GetSettingsAsync()
    {
        return await Mediator.Send(new GetSettingsQuery());
    }

    /// <summary>
    ///     Uploads settings for the calling user. Only overrides details provided and keeps the remaining details.
    /// </summary>
    /// <param name="command">The settings to persist.</param>
    /// <returns></returns>
    [HttpPost("settings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Setting))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Setting))]
    public async Task<IActionResult> PostSettingsAsync([FromBody] UploadSettingsCommand command)
    {
        var response = await Mediator.Send(command);

        return response.Existed ? Ok(response.Settings) : Created(string.Empty, response.Settings);
    }
}