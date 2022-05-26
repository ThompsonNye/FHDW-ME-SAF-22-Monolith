using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;

public class CreateConsumptionCommandHandler : IRequestHandler<CreateConsumptionCommand, Consumption>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<CreateConsumptionCommandHandler> logger;
    private readonly IMediator mediator;

    public CreateConsumptionCommandHandler(IApplicationDbContext dbContext,
        ILogger<CreateConsumptionCommandHandler> logger, IMediator mediator)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Consumption> Handle(CreateConsumptionCommand request, CancellationToken cancellationToken)
    {
        // Create the consumption entry
        var consumption = new Consumption
        {
            Id = request.Id ?? new Guid(),
            Amount = request.Amount,
            DateTime = request.DateTime,
            Distance = request.Distance,
            CarId = request.CarId,
            IgnoreInCalculation = request.IgnoreInCalculation
        };
        if (request.Id.HasValue)
        {
            if (dbContext.Consumptions.Any(x => x.Id == request.Id.Value))
                throw new DuplicateEntryException(string.Format(Resources.DuplicateEntryErrorMessage, nameof(Car),
                    request.Id.Value));
            consumption.Id = request.Id.Value;
        }

        dbContext.Consumptions.Add(consumption);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserCreatedEntity, nameof(Consumption),
            consumption.Id);
        logger.LogDebug(Resources.DebugMessageEntityCreated, nameof(Consumption),
            JsonSerializer.Serialize(consumption,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return consumption;
    }
}