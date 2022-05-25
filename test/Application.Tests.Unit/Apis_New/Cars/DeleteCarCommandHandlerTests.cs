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
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Delete;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Exceptions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class DeleteCarCommandHandlerTests
{
    private readonly DbSet<Car> _cars = Substitute.For<DbSet<Car>, IQueryable<Car>>();
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly IFixture _fixture = new Fixture();
    private readonly ILogger<DeleteCarCommandHandler> _logger = new NullLogger<DeleteCarCommandHandler>();
    private readonly DeleteCarCommandHandler _sut;

    public DeleteCarCommandHandlerTests()
    {
        _sut = new DeleteCarCommandHandler(_dbContext, _logger);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Handle_ShouldReturnUnitValue_WhenCarExists()
    {
        // Arrange
        var car = _fixture.Build<Car>().Create();
        SetupCars(car);
        var cancellationToken = new CancellationToken();
        _dbContext.SaveChangesAsync(cancellationToken).Returns(1);
        var command = new DeleteCarCommand {Id = car.Id};

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.ShouldBeOfType(typeof(Unit));
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
        _dbContext.Cars.Returns(_cars);
    }
}