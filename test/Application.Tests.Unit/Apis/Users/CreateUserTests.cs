using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Create;
using Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;
using Nuyken.VeGasCo.Backend.Infrastructure.Persistence;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Users;

public class CreateUserTests : DbTests
{
    private readonly DbConnection _connection;

    public CreateUserTests()
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection;
    }

    [Theory]
    [InlineData(false, "00000000-0000-0000-0000-000000000000", "Test", "", "")]
    [InlineData(false, "00000000-0000-0000-0000-000000000001", "", "", "")]
    [InlineData(false, "00000000-0000-0000-0000-000000000001", null, "", "")]
    [InlineData(true, "00000000-0000-0000-0000-000000000001", "Test", "", "")]
    public void TestValidation(bool expected, string id, string username, string firstname, string lastname)
    {
        var command = new CreateUserCommand
        {
            Id = Guid.Parse(id),
            Username = username,
            FirstName = firstname,
            LastName = lastname
        };
        var validator = new CreateUserCommandValidator();

        var result = validator.Validate(command);

        Assert.Equal(expected, result.IsValid);
    }


    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111111", "Test User")]
    [InlineData("11111111-1111-1111-1111-111111111111", "Test-User", "Test", "User")]
    public void TestHandler(string id, string username, string firstname = null, string lastname = null)
    {
        var guid = Guid.Parse(id);

        using var dbContext = new ApplicationDbContext(options);
        var handler = new CreateUserCommandHandler(dbContext, new NullLogger<CreateUserCommandHandler>());
        var command = new CreateUserCommand
        {
            Id = guid,
            Username = username,
            FirstName = firstname,
            LastName = lastname
        };

        var task = handler.Handle(command, CancellationToken.None);
        task.Wait();

        var createdUsers = dbContext.Users.Where(x =>
            x.Id == command.Id &&
            x.Username == command.Username &&
            x.FirstName == command.FirstName &&
            x.LastName == command.LastName
        );
        Assert.Single(createdUsers);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}