using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Delete;
using Nuyken.VeGasCo.Backend.Application.Apis.Consumptions.Get;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Consumptions;

public class DeleteConsumptionTests : DbTests
{
    private readonly DbConnection _connection;

    public DeleteConsumptionTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Theory]
    [InlineData(true, TestDataProvider.CONSUMPTION_ID)]
    [InlineData(true, TestDataProvider.CONSUMPTION_ID_OTHER_USER)]
    [InlineData(false, TestDataProvider.GUID_DEFAULT)]
    public void TestValidation(bool expectedIsValid, string id)
    {
        using var dbContext = new ApplicationDbContext(options);
        var validator = new DeleteConsumptionCommandValidator();
        var command = new DeleteConsumptionCommand
        {
            Id = Guid.Parse(id)
        };

        var validationResults = validator.Validate(command);

        Assert.Equal(expectedIsValid, validationResults.IsValid);
    }

    [Theory]
    [InlineData(TestDataProvider.CONSUMPTION_ID)]
    // The other test cases from TestValidation() above are omitted here because they cannot reach the CommandHandler as long as the validation works correctly
    public void TestCommandHandling(string id, Type exception = null)
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

        // Delete the entry with the specified id
        var handler = new DeleteConsumptionCommandHandler(dbContext, 
            new NullLogger<DeleteConsumptionCommandHandler>());
        var command = new DeleteConsumptionCommand
        {
            Id = Guid.Parse(id)
        };
        var task = handler.Handle(command, CancellationToken.None);
        task.Wait();


        // Get all consumption entries but not the deleted
        var query = new GetConsumptionsQuery();
        var getHandler = new GetConsumptionsQueryHandler(dbContext);
        var getTask = getHandler.Handle(query, CancellationToken.None);
        task.Wait();
        var result = getTask.Result;

        // Check that the deleted entry is not in the result
        Assert.True(!result.Any(x => x.Id.ToString().Equals(id, StringComparison.InvariantCulture)));
    }


    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}