using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;

public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IEnumerable<Car>>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public GetCarsQueryHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    public Task<IEnumerable<Car>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;

        var cars = dbContext.Cars
            .Where(x => x.UserId == userId)
            .AsEnumerable();

        return Task.FromResult(cars);
    }
}