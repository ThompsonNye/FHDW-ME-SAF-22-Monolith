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
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;

public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, Car>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<UpdateCarCommandHandler> logger;
    private readonly IUserAccessor userAccessor;

    public UpdateCarCommandHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor,
        ILogger<UpdateCarCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Car> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;

        var car = dbContext.Cars.Where(x => x.UserId == userId).SingleOrDefault(x => x.Id == request.Id);

        if (car is null) throw new EntityNotFoundException(nameof(Car), "null");

        request.Update(ref car);
        dbContext.Cars.Update(car);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserUpdatedEntity, userAccessor.UserName, nameof(Car), car.Id);
        logger.LogDebug(Resources.DebugMessageEntityUpdated, nameof(Car),
            JsonSerializer.Serialize(car,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return car;
    }
}