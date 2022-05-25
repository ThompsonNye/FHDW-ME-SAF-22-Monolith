using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Get;
using Nuyken.VeGasCo.Backend.Application.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class GetCarsQueryHandlerTests
{
    private readonly DbSet<Car> _cars = Substitute.For<DbSet<Car>, IQueryable<Car>>();
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly IFixture _fixture = new Fixture();
    private readonly GetCarsQueryHandler _sut;
    private readonly IUserAccessor _userAccessor = Substitute.For<IUserAccessor>();

    public GetCarsQueryHandlerTests()
    {
        _sut = new GetCarsQueryHandler(_dbContext, _userAccessor);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Handle_ShouldReturnCars_WhenUserIdMatches()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userAccessor.UserId.Returns(user.Id);
        SetupCars(user.Id);
        _dbContext.Cars.Returns(_cars);

        // Act
        var result = (await _sut.Handle(new GetCarsQuery(), new CancellationToken())).ToList();

        // Assert
        result.ShouldBeEquivalentTo(_cars.ToList());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userAccessor.UserId.Returns(user.Id);
        SetupCars(Guid.Empty);
        _dbContext.Cars.Returns(_cars);

        // Act
        var result = await _sut.Handle(new GetCarsQuery(), new CancellationToken());

        // Assert
        result.ShouldBeEmpty();
    }

    private void SetupCars(Guid userId)
    {
        var cars = new List<Car>
        {
            _fixture.Build<Car>().With(x => x.UserId, userId).Create()
        }.AsQueryable();
        ((IQueryable<Car>) _cars).Provider.Returns(cars.Provider);
        ((IQueryable<Car>) _cars).Expression.Returns(cars.Expression);
        ((IQueryable<Car>) _cars).ElementType.Returns(cars.ElementType);
        ((IQueryable<Car>) _cars).GetEnumerator().Returns(cars.GetEnumerator());
    }
}