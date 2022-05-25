using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;

public class UploadSettingsCommand : IRequest<UploadSettingsResponse>
{
    public AppColorDesign? AppColorDesign { get; set; }


    public void Update(ref Setting setting)
    {
        setting.AppColorDesign = AppColorDesign ?? setting.AppColorDesign;
    }
}