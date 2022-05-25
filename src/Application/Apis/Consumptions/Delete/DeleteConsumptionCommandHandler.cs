using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Delete;

public class DeleteConsumptionCommandHandler : IRequestHandler<DeleteConsumptionCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<DeleteConsumptionCommandHandler> logger;
    private readonly IUserAccessor userAccessor;

    public DeleteConsumptionCommandHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor,
        ILogger<DeleteConsumptionCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(DeleteConsumptionCommand request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;
        var consumption = dbContext.Consumptions.Where(x => x.UserId == userId)
            .SingleOrDefault(x => x.Id == request.Id);

        if (consumption is null) throw new EntityNotFoundException(nameof(Consumption), "null");

        dbContext.Consumptions.Remove(consumption);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserDeletedEntity, userAccessor.UserName, nameof(Consumption),
            consumption.Id);

        return Unit.Value;
    }
}