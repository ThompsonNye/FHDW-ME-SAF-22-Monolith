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

namespace Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<DeleteCarCommandHandler> logger;
    private readonly IUserAccessor userAccessor;

    public DeleteCarCommandHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor,
        ILogger<DeleteCarCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;
        var car = dbContext.Cars
            .Where(x => x.UserId == userId)
            .SingleOrDefault(x => x.Id == request.Id);

        if (car is null) throw new EntityNotFoundException(nameof(Car), "null");

        dbContext.Cars.Remove(car);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageUserDeletedEntity, userAccessor.UserName, nameof(Car), car.Id);

        return Unit.Value;
    }
}