using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;

public abstract class DbTests : IDisposable
{
    protected readonly DbContextOptions<ApplicationDbContext> options;

    public DbTests()
    {
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options;

        Seed();
    }

    public DbTests(DbContextOptions<ApplicationDbContext> options)
    {
        this.options = options;

        Seed();
    }

    public abstract void Dispose();

    private void Seed()
    {
        using var context = new ApplicationDbContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Users.AddRange(MockObjectsProvider.GetUsers());
        context.SaveChanges();
        context.Cars.AddRange(MockObjectsProvider.GetCars());
        context.SaveChanges();
        context.Consumptions.AddRange(MockObjectsProvider.GetConsumptions());
        context.SaveChanges();
    }

    protected static DbConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");

        connection.Open();

        return connection;
    }
}