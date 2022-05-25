using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Cars;

public class UpdateCarTests : DbTests
{
    private readonly DbConnection _connection;

    public UpdateCarTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Theory]
    // Invalid name
    [InlineData(false, TestDataProvider.CAR_ID, TestDataProvider.CAR_ID, "😀")]
    // Valid
    [InlineData(true, TestDataProvider.CAR_ID, TestDataProvider.CAR_ID,
        "A quick brown fox jumped over the lazy dog0123456789ßüäöÖÄÜ-_")]
    public void TestValidation(bool expected, string id, string routeId, string name = null)
    {
        var actionContextAccessor = MockObjectsProvider.GetActionContextAccessor(routeId).Object;

        var command = new UpdateCarCommand
        {
            Id = Guid.Parse(id),
            Name = name
        };

        var validator = new UpdateCarCommandValidator(actionContextAccessor);

        var result = validator.Validate(command);

        Assert.Equal(expected, result.IsValid);
    }


    [Theory]
    [InlineData(TestDataProvider.CAR_ID, "Schwachsinn")]
    [InlineData(TestDataProvider.CAR_ID_OTHER_USER, "Schwachsinn", typeof(EntityNotFoundException))]
    public void TestHandler(string id, string name, Type exception = null)
    {
        void Test()
        {
            HandlerLogic(id, name);
        }

        if (exception is null)
            Test();
        else
            ExceptionCatchWrapper.WrapCallInExceptionChecker(exception, Test);
    }

    private void HandlerLogic(string id, string name)
    {
        using var dbContext = new ApplicationDbContext(options);
        var handler =
            new UpdateCarCommandHandler(dbContext, new NullLogger<UpdateCarCommandHandler>());

        var command = new UpdateCarCommand
        {
            Id = Guid.Parse(id),
            Name = name
        };

        var task = handler.Handle(command, CancellationToken.None);
        task.Wait();

        var car = dbContext.Cars.Single(x => x.Id == command.Id);
        Assert.NotNull(car);

        if (command.Name is not null) Assert.Equal(command.Name, car.Name);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}