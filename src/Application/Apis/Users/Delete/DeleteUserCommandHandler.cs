using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public DeleteUserCommandHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;

        var user = dbContext.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null) throw new EntityNotFoundException(nameof(User), "null");

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}