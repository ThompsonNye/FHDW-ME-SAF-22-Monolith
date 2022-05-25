using MediatR;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Info.Version;

public class GetVersionQuery : IRequest<GetVersionInfoResponse>
{
}