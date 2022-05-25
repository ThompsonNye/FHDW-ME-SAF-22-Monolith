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

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;

public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, Car>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<UpdateCarCommandHandler> logger;

    public UpdateCarCommandHandler(IApplicationDbContext dbContext,
        ILogger<UpdateCarCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Car> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var car = dbContext.Cars
            .SingleOrDefault(x => x.Id == request.Id);

        if (car is null) throw new EntityNotFoundException(nameof(Car), "null");

        request.Update(ref car);
        dbContext.Cars.Update(car);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserUpdatedEntity, nameof(Car), car.Id);
        logger.LogDebug(Resources.DebugMessageEntityUpdated, nameof(Car),
            JsonSerializer.Serialize(car,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return car;
    }
}