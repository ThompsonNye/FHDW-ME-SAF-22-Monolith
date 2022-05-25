using Microsoft.AspNetCore.Builder;
using Nuyken.VeGasCo.Backend.WebApi.Common.Middleware;

namespace Nuyken.VeGasCo.Backend.WebApi.Common;

public static class DependencyInjection
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}