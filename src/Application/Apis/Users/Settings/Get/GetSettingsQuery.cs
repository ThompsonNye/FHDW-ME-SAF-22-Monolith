using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Get;

public class GetSettingsQuery : IRequest<Setting>
{
}