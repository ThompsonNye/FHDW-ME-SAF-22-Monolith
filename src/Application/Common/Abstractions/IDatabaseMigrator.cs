using System.Threading.Tasks;

namespace Nuyken.VeGasCo.Backend.Application.Common.Abstractions;

public interface IDatabaseMigrator
{
    public Task MigrateAsync();
}