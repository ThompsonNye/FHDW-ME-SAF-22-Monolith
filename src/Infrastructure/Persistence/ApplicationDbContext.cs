using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Consumption> Consumptions { get; set; }

    public DbSet<Car> Cars { get; set; }

    public void EnsureDateTimeKindUTC(ModelBuilder builder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless) continue;

            foreach (var prop in entityType.GetProperties())
            {
                if (prop.ClrType == typeof(DateTime))
                {
                    prop.SetValueConverter(dateTimeConverter);
                    continue;
                }

                if (prop.ClrType == typeof(DateTime?)) prop.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        EnsureDateTimeKindUTC(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var name = Assembly.GetExecutingAssembly().FullName ?? "InMemDefaultDb";
        optionsBuilder.UseInMemoryDatabase(name);
    }
}