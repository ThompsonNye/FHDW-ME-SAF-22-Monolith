using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;

public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IEnumerable<Car>>
{
    private readonly IApplicationDbContext dbContext;

    public GetCarsQueryHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<IEnumerable<Car>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        var cars = dbContext.Cars
            .AsEnumerable();

        return Task.FromResult(cars);
    }
}