using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Moq;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;

public static class MockObjectsProvider
{
    /// <summary>
    ///     Creates and returns a mocked <see cref="IUserAccessor" /> objects. The caller can optionally pass the
    ///     <paramref name="id" /> and <paramref name="username" /> to use.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public static Mock<IUserAccessor> GetUserAccessMock(string id = TestDataProvider.USER_ID,
        string username = "thomas.nuyken")
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));

        if (string.IsNullOrEmpty(username))
            throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));

        var mock = new Mock<IUserAccessor>();

        var claimUserId = new Claim(ClaimTypes.NameIdentifier, id);
        var claimUserName = new Claim("preferred_username", username);
        var identity = new ClaimsIdentity(new List<Claim> {claimUserId, claimUserName});
        var user = new ClaimsPrincipal(identity);

        mock.Setup(x => x.User).Returns(user);
        mock.Setup(x => x.UserId).Returns(Guid.Parse(id));
        mock.Setup(x => x.UserName).Returns(username);

        return mock;
    }

    public static Mock<IActionContextAccessor> GetActionContextAccessor(string routeId)
    {
        return GetActionContextAccessor(new Dictionary<string, string> {{"id", routeId}});
    }

    public static Mock<IActionContextAccessor> GetActionContextAccessor(Dictionary<string, string> routeData = null)
    {
        if (routeData is null) routeData = new Dictionary<string, string> {{"id", ""}};

        var actionContext = new ActionContext
        {
            RouteData = new RouteData(new RouteValueDictionary(routeData))
        };

        var mockActionContextAccessor = new Mock<IActionContextAccessor>();
        mockActionContextAccessor.Setup(x => x.ActionContext).Returns(actionContext);

        return mockActionContextAccessor;
    }

    public static List<Car> GetCars()
    {
        return new List<Car>
        {
            new()
            {
                Id = Guid.Parse(TestDataProvider.CAR_ID),
                UserId = Guid.Parse(TestDataProvider.USER_ID),
                Name = "Example Car"
            },
            new()
            {
                Id = Guid.Parse(TestDataProvider.CAR_ID_OTHER_USER),
                UserId = Guid.Parse(TestDataProvider.USER_ID_OTHER_USER),
                Name = "Example Car"
            }
        };
    }

    public static List<Consumption> GetConsumptions()
    {
        return new List<Consumption>
        {
            new()
            {
                Id = Guid.Parse(TestDataProvider.CONSUMPTION_ID),
                Amount = 1,
                Distance = 1,
                DateTime = new DateTime(2021, 08, 01),
                CarId = Guid.Parse(TestDataProvider.CAR_ID),
                UserId = Guid.Parse(TestDataProvider.USER_ID)
            },
            new()
            {
                Id = Guid.Parse(TestDataProvider.CONSUMPTION_ID_OTHER_USER),
                Amount = 3,
                Distance = 3,
                DateTime = new DateTime(2021, 08, 01),
                CarId = Guid.Parse(TestDataProvider.CAR_ID_OTHER_USER),
                UserId = Guid.Parse(TestDataProvider.USER_ID_OTHER_USER)
            }
        };
    }

    public static List<User> GetUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = Guid.Parse(TestDataProvider.USER_ID),
                FirstName = "Thomas",
                LastName = "Nuyken",
                Username = "thomas.nuyken"
            },
            new()
            {
                Id = Guid.Parse(TestDataProvider.USER_ID_OTHER_USER),
                FirstName = "Thomas",
                LastName = "Nuyken",
                Username = "nuykent@gmail.com"
            }
        };
    }
}