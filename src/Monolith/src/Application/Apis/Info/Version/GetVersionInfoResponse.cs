using System;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Info.Version;

public class GetVersionInfoResponse
{
    public string FullVersion { get; set; }

    public string CommitId { get; set; }

    public DateTime CommitDate { get; set; }

    public string Version { get; set; }

    public string Environment { get; set; }
}