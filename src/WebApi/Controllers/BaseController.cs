using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Nuyken.VeGasCo.Backend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public abstract class BaseController : ControllerBase
{
    private IMediator mediator;

    protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();
}