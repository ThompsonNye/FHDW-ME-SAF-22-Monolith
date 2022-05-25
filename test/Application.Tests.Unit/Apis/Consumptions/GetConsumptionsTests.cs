using System;
using System.Data.Common;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Get;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Consumptions;

public class GetConsumptionsTests : DbTests
{
    private readonly DbConnection _connection;

    public GetConsumptionsTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    /// <summary>
    ///     Checks if all returned consumptions contain the <see cref="Car" /> and <see cref="User" /> objects and also the
    ///     <see cref="Consumption.CarName" /> and <see cref="Consumption.UserName" /> properties.
    /// </summary>
    [Fact]
    public void TestCarAndUsernameInConsumptionEntries()
    {
        using var dbContext = new ApplicationDbContext(options);

        var query = new GetConsumptionsQuery();

        var handler = new GetConsumptionsQueryHandler(dbContext);
        var task = handler.Handle(query, CancellationToken.None);
        task.Wait();
        var result = task.Result;

        Assert.NotEmpty(result);
        foreach (var c in result)
        {
            Assert.NotNull(c.Car);
            Assert.False(string.IsNullOrEmpty(c.CarName));
        }
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}