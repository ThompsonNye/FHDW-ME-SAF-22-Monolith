using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Common.Options;
using Nuyken.VeGasCo.Backend.Infrastructure.Properties;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly IHost _host;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(ILogger<DatabaseMigrator> logger, IHost host)
    {
        _logger = logger;
        _host = host;
    }

    public async Task MigrateAsync()
    {
        using var scope = _host.Services.CreateScope();
        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (dbOptions is null)
        {
            _logger?.LogInformation(Resources.InfoMessageFetchingDbOptions);
            return;
        }

        if (!dbOptions.AutoMigrate)
        {
            _logger?.LogInformation(Resources.InfoMessageAutoMigrationDisabled);
            return;
        }

        await RunMigrationAsync(scope);
    }

    private async Task RunMigrationAsync(IServiceScope scope)
    {
        _logger?.LogInformation(Resources.InfoMessageMigratingDatabase);

        DbContext context;

        try
        {
            context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>() as DbContext;
        }
        catch (Exception ex)
        {
            var message = string.Format(Resources.ErrorMessageUnableToRetrieve, nameof(IApplicationDbContext));
            throw new MigrationImpossibleException(message, ex);
        }

        if (!context!.Database.IsInMemory())
            try
            {
                await context.Database.MigrateAsync();
                _logger?.LogInformation(Resources.InfoMessageMigrationSuccessful);
            }
            catch (Exception ex)
            {
                throw new MigrationImpossibleException(Resources.ErrorMessageUnexpectedMigrationError, ex);
            }
    }
}