using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Moq;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;

public static class MockObjectsProvider
{
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
                Name = "Example Car"
            },
            new()
            {
                Id = Guid.Parse(TestDataProvider.CAR_ID_OTHER_USER),
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
                CarId = Guid.Parse(TestDataProvider.CAR_ID)
            },
            new()
            {
                Id = Guid.Parse(TestDataProvider.CONSUMPTION_ID_OTHER_USER),
                Amount = 3,
                Distance = 3,
                DateTime = new DateTime(2021, 08, 01),
                CarId = Guid.Parse(TestDataProvider.CAR_ID_OTHER_USER)
            }
        };
    }
}