using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<DeleteCarCommandHandler> logger;

    public DeleteCarCommandHandler(IApplicationDbContext dbContext,
        ILogger<DeleteCarCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var car = dbContext.Cars
            .SingleOrDefault(x => x.Id == request.Id);

        if (car is null) throw new EntityNotFoundException(nameof(Car), "null");

        dbContext.Cars.Remove(car);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserDeletedEntity, nameof(Car), car.Id);

        return Unit.Value;
    }
}