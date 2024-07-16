﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;

public class UpdateConsumptionCommandHandler : IRequestHandler<UpdateConsumptionCommand, Consumption>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<UpdateConsumptionCommandHandler> logger;
    private readonly IUserAccessor userAccessor;

    public UpdateConsumptionCommandHandler(IUserAccessor userAccessor, IApplicationDbContext dbContext,
        ILogger<UpdateConsumptionCommandHandler> logger)
    {
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Consumption> Handle(UpdateConsumptionCommand request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;
        var consumption = dbContext.Consumptions
            .Where(x => x.UserId == userId)
            .SingleOrDefault(x => x.Id == request.Id);

        if (consumption is null) throw new EntityNotFoundException(nameof(Consumption), "null");

        request.Update(ref consumption);
        dbContext.Consumptions.Update(consumption);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserUpdatedEntity, userAccessor.UserName, nameof(Consumption),
            consumption.Id);
        logger.LogDebug(Resources.DebugMessageEntityUpdated, nameof(Consumption),
            JsonSerializer.Serialize(consumption,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return consumption;
    }
}