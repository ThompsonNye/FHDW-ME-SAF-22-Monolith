using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Cars;

public class DeleteCarTests : DbTests
{
    private readonly DbConnection _connection;

    public DeleteCarTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Theory]
    [InlineData(false, null)]
    [InlineData(false, "")]
    [InlineData(false, "00000000-0000-0000-0000-000000000000")]
    [InlineData(false, "abcdefgh-ihij-klmn-opqr-stuvwxyz0123")]
    [InlineData(true, "abcdef01-2345-6789-abcd-ef0123456789")]
    public void TestValidation(bool expected, string id)
    {
        var idCanBeParsed = Guid.TryParse(id, out var guid);

        bool Test()
        {
            var command = new DeleteCarCommand
            {
                Id = guid
            };
            var validator = new DeleteCarCommandValidator();
            var result = validator.Validate(command);
            return result.IsValid;
        }

        Assert.Equal(expected, idCanBeParsed && Test());
    }

    [Theory]
    [InlineData(TestDataProvider.CAR_ID)]
    [InlineData(TestDataProvider.CAR_ID_OTHER_USER, typeof(EntityNotFoundException))]
    [InlineData("00000000-0000-0000-0000-000000000000", typeof(EntityNotFoundException))]
    public void TestHandler(string id, Type exception = null)
    {
        void Test()
        {
            HandlerTestLogic(id);
        }

        if (exception is null)
            Test();
        else
            ExceptionCatchWrapper.WrapCallInExceptionChecker(exception, Test);
    }

    private void HandlerTestLogic(string id)
    {
        using var dbContext = new ApplicationDbContext(options);
        var userAccessor = MockObjectsProvider.GetUserAccessMock().Object;
        var carId = Guid.Parse(id);
        var command = new DeleteCarCommand
        {
            Id = carId
        };
        var handler =
            new DeleteCarCommandHandler(dbContext, userAccessor, new NullLogger<DeleteCarCommandHandler>());

        var countBefore = dbContext.Cars.Count();

        var task = handler.Handle(command, CancellationToken.None);
        task.Wait();

        var countAfter = dbContext.Cars.Count();

        var carDoesNotExistsAnymore = !dbContext.Cars.Any(x => x.Id == carId);

        Assert.Equal(countBefore - 1, countAfter);
        Assert.True(carDoesNotExistsAnymore);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}