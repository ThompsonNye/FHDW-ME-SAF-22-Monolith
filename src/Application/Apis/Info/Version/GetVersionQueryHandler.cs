using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Info.Version;

public class GetVersionQueryHandler : IRequestHandler<GetVersionQuery, GetVersionInfoResponse>
{
    private readonly IWebHostEnvironment hostingEnvironment;

    public GetVersionQueryHandler(IWebHostEnvironment hostingEnvironment)
    {
        this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    }

    public Task<GetVersionInfoResponse> Handle(GetVersionQuery request, CancellationToken cancellationToken)
    {
        var versionInfo = new GetVersionInfoResponse
        {
            CommitId = ThisAssembly.GitCommitId,
            CommitDate = ThisAssembly.GitCommitDate,
            FullVersion = ThisAssembly.AssemblyInformationalVersion,
            Version = ThisAssembly.AssemblyVersion,
            Environment = hostingEnvironment.EnvironmentName ?? string.Empty
        };
        return Task.FromResult(versionInfo);
    }
}