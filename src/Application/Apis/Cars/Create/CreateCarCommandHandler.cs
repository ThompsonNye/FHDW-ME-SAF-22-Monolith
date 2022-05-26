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

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;

public class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, Car>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<CreateCarCommandHandler> logger;
    private readonly IMediator mediator;

    public CreateCarCommandHandler(IApplicationDbContext dbContext,
        ILogger<CreateCarCommandHandler> logger, IMediator mediator)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Car> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var car = new Car
        {
            Id = request.Id ?? new Guid(),
            Name = request.Name
        };
        if (request.Id.HasValue)
        {
            if (dbContext.Cars.Any(x => x.Id == request.Id.Value))
                throw new DuplicateEntryException(string.Format(Resources.DuplicateEntryErrorMessage, nameof(Car),
                    request.Id.Value));
            car.Id = request.Id.Value;
        }

        dbContext.Cars.Add(car);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserCreatedEntity, nameof(Car), car.Id);
        logger.LogDebug(Resources.DebugMessageEntityCreated, nameof(Car),
            JsonSerializer.Serialize(car,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return car;
    }
}