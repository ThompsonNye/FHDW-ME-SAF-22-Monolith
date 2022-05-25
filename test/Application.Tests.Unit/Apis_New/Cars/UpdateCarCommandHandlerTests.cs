using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class UpdateCarCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly IFixture _fixture = new Fixture();
    private readonly ILogger<UpdateCarCommandHandler> _logger = new NullLogger<UpdateCarCommandHandler>();
    private readonly UpdateCarCommandHandler _sut;
    private readonly IUserAccessor _userAccessor = Substitute.For<IUserAccessor>();

    public UpdateCarCommandHandlerTests()
    {
        _sut = new UpdateCarCommandHandler(_dbContext, _userAccessor, _logger);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Handle_ShouldReturnCar_WhenDetailsAreValid()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userAccessor.UserId.Returns(user.Id);
        _userAccessor.UserName.Returns(user.Username);
        var car = _fixture.Build<Car>().With(x => x.UserId, user.Id).Create();
        SetupCars(car);
        var command = _fixture.Build<UpdateCarCommand>()
            .With(x => x.Id, car.Id).Create();

        // Act
        var result = await _sut.Handle(command, new CancellationToken());

        // Assert
        result.Name.ShouldBe(command.Name);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenDetailsDoNotMatch()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userAccessor.UserId.Returns(user.Id);
        _userAccessor.UserName.Returns(user.Username);
        var car = _fixture.Create<Car>();
        SetupCars(car);
        var command = _fixture.Create<UpdateCarCommand>();

        // Act & Assert
        _sut.Invoking(async handler => await handler.Handle(command, new CancellationToken()))
            .ShouldThrow<EntityNotFoundException>();
    }

    private void SetupCars(Car car)
    {
        var cars = new[]
        {
            car
        }.AsQueryable();
        var carsDbSet = Substitute.For<DbSet<Car>, IQueryable<Car>>();
        ((IQueryable<Car>) carsDbSet).Provider.Returns(cars.Provider);
        ((IQueryable<Car>) carsDbSet).Expression.Returns(cars.Expression);
        ((IQueryable<Car>) carsDbSet).ElementType.Returns(cars.ElementType);
        ((IQueryable<Car>) carsDbSet).GetEnumerator().Returns(cars.GetEnumerator());
        _dbContext.Cars.Returns(carsDbSet);
    }
}