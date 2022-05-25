using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Create;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Consumptions;

public class CreateConsumptionTests : DbTests
{
    private readonly DbConnection _connection;

    public CreateConsumptionTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    /// <summary>
    ///     Checks whether the <see cref="CreateConsumptionCommandValidator" /> correctly validates data.
    /// </summary>
    /// <param name="expectedIsValid"></param>
    /// <param name="distance"></param>
    /// <param name="amount"></param>
    /// <param name="carId"></param>
    /// <param name="dateTime"></param>
    /// <param name="exceptionType"></param>
    [Theory]
    [InlineData(true, 1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", "2021-08-01T00:00:00Z")]
    [InlineData(true, 0, 0, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", "2021-08-01T00:00:00Z")]
    // Specified cars exist but belong to the wrong user
    [InlineData(false, 1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0c", "2021-08-01T00:00:00Z")]
    // Invalid distance or amount
    [InlineData(false, -1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", "2021-08-01T00:00:00Z")]
    [InlineData(false, 1, -1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", "2021-08-01T00:00:00Z")]
    [InlineData(false, -1, -1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", "2021-08-01T00:00:00Z")]
    [InlineData(false, -1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0e", "2021-08-01T00:00:00Z")]
    [InlineData(false, 1, -1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0e", "2021-08-01T00:00:00Z")]
    [InlineData(false, -1, -1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0e", "2021-08-01T00:00:00Z")]
    // Car does not exist
    [InlineData(false, 1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0e", "2021-08-01T00:00:00Z")]
    // Car ID not a valid GUID
    [InlineData(false, 1, 1, "", "2021-08-01T00:00:00Z", typeof(FormatException))]
    [InlineData(false, 1, 1, null, "2021-08-01T00:00:00Z", typeof(ArgumentNullException))]
    public void TestValidation(bool expectedIsValid, double distance, double amount, string carId, string dateTime,
        Type exception = null)
    {
        void Test()
        {
            TestValidationLogic(expectedIsValid, distance, amount, carId, dateTime);
        }

        if (exception is null)
            Test();
        else
            ExceptionCatchWrapper.WrapCallInExceptionChecker(exception, Test);
    }

    private void TestValidationLogic(bool expectedIsValid, double distance, double amount, string carId,
        string dateTime)
    {
        using var dbContext = new ApplicationDbContext(options);
        var userAccessMock = MockObjectsProvider.GetUserAccessMock();
        var validator = new CreateConsumptionCommandValidator(userAccessMock.Object, dbContext);

        var command = new CreateConsumptionCommand
        {
            Distance = distance,
            Amount = amount,
            CarId = Guid.Parse(carId),
            DateTime = DateTime.Parse(dateTime)
        };

        var result = validator.Validate(command);

        Assert.Equal(expectedIsValid, result.IsValid);
    }

    /// <summary>
    ///     Checks whether the <see cref="CreateConsumptionCommandValidator" /> correctly validates a date in the future.
    /// </summary>
    [Fact]
    public void TestFutureDate()
    {
        var futureDate = DateTime.UtcNow.AddDays(1).ToString("o");
        TestValidation(false, 1, 1, "24b4a92e-dcb5-42b8-aad8-5cb07b282a0a", futureDate);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}