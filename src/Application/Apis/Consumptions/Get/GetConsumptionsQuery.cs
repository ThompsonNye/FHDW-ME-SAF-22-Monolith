using System.Collections.Generic;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Get;

public class GetConsumptionsQuery : IRequest<IEnumerable<Consumption>>
{
}