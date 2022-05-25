using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;

namespace Nuyken.VeGasCo.Backend.Application.Common.Behaviours;

public class RequestNotNullBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request == null)
            throw new ValidationException
            {
                Failures =
                {
                    {string.Empty, new[] {Resources.ErrorMessageBehaviourNoData}}
                }
            };

        return next();
    }
}