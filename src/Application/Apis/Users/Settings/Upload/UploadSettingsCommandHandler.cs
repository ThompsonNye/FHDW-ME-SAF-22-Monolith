using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Properties;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;

public class UploadSettingsCommandHandler : IRequestHandler<UploadSettingsCommand, UploadSettingsResponse>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<UploadSettingsCommandHandler> logger;
    private readonly IUserAccessor userAccessor;

    public UploadSettingsCommandHandler(IApplicationDbContext dbContext, IUserAccessor userAccessor,
        ILogger<UploadSettingsCommandHandler> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UploadSettingsResponse> Handle(UploadSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = await EnsureUserAsync(cancellationToken);

        var setting = dbContext.Settings.FirstOrDefault(x => x.UserId == userId);

        var settingsExisted = setting != null;
        setting ??= new Setting {UserId = userId};

        request.Update(ref setting);

        if (settingsExisted)
            dbContext.Settings.Update(setting);
        else
            dbContext.Settings.Add(setting);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(Resources.InfoMessageSavedSettingsForUser, userId);
        logger.LogDebug(Resources.DebugMessageUploadedSettingsForUser, userId, JsonSerializer.Serialize(setting));

        return new UploadSettingsResponse {Existed = settingsExisted, Settings = setting};
    }

    private async Task<Guid> EnsureUserAsync(CancellationToken cancellationToken)
    {
        var userId = userAccessor.UserId;
        if (!dbContext.Users.Any(x => x.Id == userId))
        {
            var user = new User
            {
                Id = userId,
                FirstName = userAccessor.FirstName,
                LastName = userAccessor.LastName,
                Username = userAccessor.UserName
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return userId;
    }
}