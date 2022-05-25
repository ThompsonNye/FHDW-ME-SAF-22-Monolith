using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Get;

public class GetConsumptionsQueryHandler : IRequestHandler<GetConsumptionsQuery, IEnumerable<Consumption>>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public GetConsumptionsQueryHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor)
    {
        this.dbContext = dbContext;
        this.userAccessor = userAccessor;
    }

    public Task<IEnumerable<Consumption>> Handle(GetConsumptionsQuery request, CancellationToken cancellationToken)
    {
        var consumptions = dbContext.Consumptions
            .Include(x => x.User)
            .Include(x => x.Car)
            .Where(x => x.UserId == userAccessor.UserId)
            .AsEnumerable();

        return Task.FromResult(consumptions);
    }
}