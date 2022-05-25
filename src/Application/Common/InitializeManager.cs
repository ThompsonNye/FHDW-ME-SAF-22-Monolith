using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Common;

public class InitializeManager
{
    private readonly IHost _host;

    public InitializeManager(IHost host)
    {
        _host = host;
    }

    public async Task InitializeAsync()
    {
        var scope = _host.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
        await migrator.MigrateAsync();
    }
}