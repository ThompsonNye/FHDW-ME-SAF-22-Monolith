using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Update;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Consumptions;

public class UpdateConsumptionTests : DbTests
{
    private readonly DbConnection _connection;


    public UpdateConsumptionTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    /// <summary>
    ///     Checks whether the <see cref="UpdateConsumptionCommandValidator" /> correctly validates data.
    /// </summary>
    /// <param name="expectedIsValid"></param>
    /// <param name="distance"></param>
    /// <param name="amount"></param>
    /// <param name="carId"></param>
    /// <param name="dateTime"></param>
    /// <param name="deleted"></param>
    /// <param name="updateDeleted"></param>
    /// <param name="exceptionType"></param>
    [Theory]
    // ID and routeId mismatch
    // ID and routeId mismatch and id does not belong to the user
    [InlineData(false, TestDataProvider.CONSUMPTION_ID_OTHER_USER, TestDataProvider.CONSUMPTION_ID, 0, 0,
        TestDataProvider.CAR_ID, TestDataProvider.DATE_PAST)]
    // Default GUID
    [InlineData(false, TestDataProvider.GUID_DEFAULT, TestDataProvider.CONSUMPTION_ID, 0, 0,
        TestDataProvider.CAR_ID, TestDataProvider.DATE_PAST)]
    // Invalid distance
    [InlineData(false, TestDataProvider.CONSUMPTION_ID, TestDataProvider.CONSUMPTION_ID, -1, 0,
        TestDataProvider.CAR_ID, TestDataProvider.DATE_PAST)]
    // Invalid amount
    [InlineData(false, TestDataProvider.CONSUMPTION_ID, TestDataProvider.CONSUMPTION_ID, 0, -1,
        TestDataProvider.CAR_ID, TestDataProvider.DATE_PAST)]
    // Car not owned by user or default GUID
    [InlineData(false, TestDataProvider.CONSUMPTION_ID, TestDataProvider.CONSUMPTION_ID, 0, 0,
        TestDataProvider.CAR_ID_OTHER_USER, TestDataProvider.DATE_PAST)]
    [InlineData(false, TestDataProvider.CONSUMPTION_ID, TestDataProvider.CONSUMPTION_ID, 0, 0,
        TestDataProvider.GUID_DEFAULT, TestDataProvider.DATE_PAST)]

    // All valid
    [InlineData(true, TestDataProvider.CONSUMPTION_ID, TestDataProvider.CONSUMPTION_ID, 0, 0,
        TestDataProvider.CAR_ID, TestDataProvider.DATE_PAST)]
    public void TestValidation(bool expectedIsValid, string id, string idInRoute, double distance, double amount,
        string carId, string dateTime, Type exception = null)
    {
        void Test()
        {
            ValidationTestLogic(expectedIsValid, id, idInRoute, distance, amount, carId, dateTime);
        }

        if (exception is null)
            Test();
        else
            ExceptionCatchWrapper.WrapCallInExceptionChecker(exception, Test);
    }

    private void ValidationTestLogic(bool expectedIsValid, string id, string idInRoute, double distance,
        double amount, string carId, string dateTime)
    {
        using var dbContext = new ApplicationDbContext(options);
        var routeData = new Dictionary<string, string>
        {
            {"id", idInRoute}
        };
        var actionContextAccessor = MockObjectsProvider.GetActionContextAccessor(routeData).Object;
        var validator = new UpdateConsumptionCommandValidator(actionContextAccessor, dbContext);
        var command = new UpdateConsumptionCommand
        {
            Id = Guid.Parse(id),
            Amount = amount,
            Distance = distance,
            CarId = Guid.Parse(carId),
            DateTime = DateTime.Parse(dateTime)
        };

        var result = validator.Validate(command);

        Assert.Equal(expectedIsValid, result.IsValid);
    }


    [Fact]
    public void TestValidationWithFutureDate()
    {
        var expectedIsValid = false;
        var id = TestDataProvider.CONSUMPTION_ID;
        var idInRoute = TestDataProvider.CONSUMPTION_ID;
        double distance = 0;
        double amount = 0;
        var carId = TestDataProvider.CAR_ID;
        var dateTime = DateTime.UtcNow.AddDays(1).ToString("O");

        TestValidation(expectedIsValid, id, idInRoute, distance, amount, carId, dateTime);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}