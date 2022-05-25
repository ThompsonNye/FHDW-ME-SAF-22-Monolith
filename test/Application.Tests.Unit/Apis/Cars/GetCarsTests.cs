using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Cars;

public class GetCarsTests : DbTests
{
    private readonly DbConnection _connection;

    public GetCarsTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Theory]
    [InlineData(1)]
    public void TestHandler(int expectedCount)
    {
        using var dbContext = new ApplicationDbContext(options);

        var query = new GetCarsQuery();
        var handler = new GetCarsQueryHandler(dbContext);

        var task = handler.Handle(query, CancellationToken.None);
        task.Wait();
        var result = task.Result;

        Assert.Equal(expectedCount, result.Count());
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}