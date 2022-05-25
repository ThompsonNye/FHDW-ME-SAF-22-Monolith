using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.WebApi.Common.Models;
using Nuyken.VeGasCo.Backend.WebApi.Properties;

namespace Nuyken.VeGasCo.Backend.WebApi.Common.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly ILogger<CustomExceptionHandlerMiddleware> logger;
    private readonly RequestDelegate next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var handled = await HandleExceptionAsync(context, ex);
            if (!handled)
            {
                logger.LogError(ex,
                    Resources.ExceptionMiddlewareUnexpectedError,
                    nameof(CustomExceptionHandlerMiddleware));
                throw;
            }
        }
    }

    /// <summary>
    ///     Handles the exception if possible.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns>Whether the exception was handled.</returns>
    private async Task<bool> HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, Resources.ExceptionMiddlewareUnexpectedErrorWithUrl,
            context.Request.GetDisplayUrl());

        if (context.Response.HasStarted)
            // Since we cannot modify the response anymore, return false so the calling method throws the exception further
            return false;

        var error = new ErrorResponse();

        ProcessException(exception, error, out var code);

        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonConvert.SerializeObject(error));

        return true;
    }

    private void ProcessException(Exception exception, ErrorResponse response, out int statusCode)
    {
        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                response.Message = Resources.ErrorMessageInvalidData;
                response.Error = validationException.Failures;
                break;
            case EntityNotFoundException _:
                statusCode = StatusCodes.Status404NotFound;
                response.Message = Resources.ErrorMessageEntityNotFound;
                break;
            case DuplicateEntryException _:
                statusCode = StatusCodes.Status409Conflict;
                response.Message = Resources.DuplicateEntryErrorMessage;
                break;
            default:
                logger.LogError(exception, Resources.ErrorMessageUnexpectedError);
                statusCode = StatusCodes.Status500InternalServerError;
                response.Message = Resources.ErrorMessageUnexpectedError;
                break;
        }
    }
}