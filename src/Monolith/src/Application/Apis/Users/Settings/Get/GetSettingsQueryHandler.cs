using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Get;

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, Setting>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IUserAccessor userAccessor;

    public GetSettingsQueryHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    public Task<Setting> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;

        var setting = dbContext.Settings.FirstOrDefault(x => x.UserId == userId);

        if (setting is null) throw new EntityNotFoundException(nameof(Setting), "null");

        return Task.FromResult(setting);
    }
}