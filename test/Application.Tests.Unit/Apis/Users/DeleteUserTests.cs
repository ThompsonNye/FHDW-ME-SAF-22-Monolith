using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Delete;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Users;

public class DeleteUserTests : DbTests
{
    private readonly DbConnection _connection;

    public DeleteUserTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Fact]
    public void TestHandler()
    {
        using var dbContext = new ApplicationDbContext(options);
        var userAccessor = MockObjectsProvider.GetUserAccessMock().Object;

        BeforeDeletionChecks(dbContext);

        var command = new DeleteUserCommand();
        var handler = new DeleteUserCommandHandler(dbContext, userAccessor);

        var task = handler.Handle(command, CancellationToken.None);
        task.Wait();

        AfterDeletionChecks(dbContext);
    }

    private static void BeforeDeletionChecks(IApplicationDbContext dbContext)
    {
        var users = dbContext.Users.Where(x => x.Id == Guid.Parse(TestDataProvider.USER_ID));
        Assert.NotEmpty(users);

        var cars = dbContext.Cars.Where(x =>
            x.Id == Guid.Parse(TestDataProvider.CAR_ID));
        Assert.Equal(1, cars.Count());

        var consumptions = dbContext.Consumptions.Where(x =>
            x.Id == Guid.Parse(TestDataProvider.CONSUMPTION_ID));
        Assert.Equal(1, consumptions.Count());
    }

    private static void AfterDeletionChecks(IApplicationDbContext dbContext)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataProvider.USER_ID));
        Assert.Null(user);

        var cars = dbContext.Cars.Where(x =>
            x.Id == Guid.Parse(TestDataProvider.CAR_ID));
        Assert.Empty(cars);

        var consumptions = dbContext.Consumptions.Where(x =>
            x.Id == Guid.Parse(TestDataProvider.CONSUMPTION_ID));
        Assert.Empty(consumptions);
    }


    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}