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
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<CreateUserCommandHandler> logger;

    public CreateUserCommandHandler(IApplicationDbContext dbContext, ILogger<CreateUserCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (dbContext.Users.Any(x => x.Id == request.Id))
            throw new InvalidOperationException(string.Format(Resources.ErrorMessageUserWithIdAlreadyExists,
                request.Id));

        var user = new User
        {
            Id = request.Id,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Using debug message with info log here because this template works best for creating a user entry
        logger.LogInformation(Resources.DebugMessageEntityCreated, nameof(User),
            JsonSerializer.Serialize(user,
                new JsonSerializerOptions {ReferenceHandler = ReferenceHandler.Preserve}));

        return Unit.Value;
    }
}