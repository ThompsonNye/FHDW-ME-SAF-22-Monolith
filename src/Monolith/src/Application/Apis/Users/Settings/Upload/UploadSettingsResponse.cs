using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;

public class UploadSettingsResponse
{
    public bool Existed { get; set; }

    public Setting Settings { get; set; }
}