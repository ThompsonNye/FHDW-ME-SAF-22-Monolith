using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nuyken.VeGasCo.Backend.Application.Apis.Info.Version;

namespace Nuyken.VeGasCo.Backend.WebApi.Controllers;

public class InfoController : BaseController
{
    /// <summary>
    ///     Returns information about the server.
    /// </summary>
    /// <returns></returns>
    [HttpGet("server")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetVersionInfoResponse>> GetServerInfoAsync()
    {
        var query = new GetVersionQuery();
        return await Mediator.Send(query);
    }
}