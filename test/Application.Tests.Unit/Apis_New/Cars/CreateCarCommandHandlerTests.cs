using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class CreateCarCommandHandlerTests
{
    private readonly DbSet<Car> _cars = Substitute.For<DbSet<Car>, IQueryable<Car>>();
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly IFixture _fixture = new Fixture();
    private readonly ILogger<CreateCarCommandHandler> _logger = new NullLogger<CreateCarCommandHandler>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateCarCommandHandler _sut;

    public CreateCarCommandHandlerTests()
    {
        _sut = new CreateCarCommandHandler(_dbContext, _logger, _mediator);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Handle_ShouldReturnCar_WhenDetailsAreValid()
    {
        // Arrange
        var car = _fixture.Create<Car>();
        SetupCars(car);
        _dbContext.Cars.Returns(_cars);
        var cancellationToken = new CancellationToken();
        _dbContext.SaveChangesAsync(cancellationToken).Returns(1);
        var command = new CreateCarCommand
        {
            Name = car.Name
        };

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Name.ShouldBe(command.Name);
        _dbContext.Cars.ReceivedWithAnyArgs();
    }

    [Fact]
    public void Handle_ShouldThrowDuplicateException_WhenCarIdExists()
    {
        var car = _fixture.Create<Car>();
        SetupCars(car);
        _dbContext.Cars.Returns(_cars);
        var cancellationToken = new CancellationToken();
        _dbContext.SaveChangesAsync(cancellationToken).Returns(0);
        var command = new CreateCarCommand
        {
            Id = car.Id,
            Name = car.Name
        };

        // Act & Assert
        _sut.Invoking(async handler => await handler.Handle(command, cancellationToken))
            .ShouldThrow<DuplicateEntryException>();
    }

    private void SetupCars(Car car)
    {
        var cars = new List<Car>
        {
            car
        }.AsQueryable();
        ((IQueryable<Car>) _cars).Provider.Returns(cars.Provider);
        ((IQueryable<Car>) _cars).Expression.Returns(cars.Expression);
        ((IQueryable<Car>) _cars).ElementType.Returns(cars.ElementType);
        ((IQueryable<Car>) _cars).GetEnumerator().Returns(cars.GetEnumerator());
        _cars.Add(car).ReturnsNull();
    }
}