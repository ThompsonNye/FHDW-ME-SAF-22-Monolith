using System.Collections.Generic;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;

public class GetCarsQuery : IRequest<IEnumerable<Car>>
{
}