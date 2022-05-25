using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Consumption> Consumptions { get; set; }

    DbSet<Car> Cars { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<Setting> Settings { get; set; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void EnsureDateTimeKindUTC(ModelBuilder builder);
}