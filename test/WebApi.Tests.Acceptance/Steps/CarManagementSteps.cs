using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Abstractions;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Steps;

[Binding]
public sealed class CarManagementSteps
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public CarManagementSteps(ScenarioContext scenarioContext, HttpClient httpClient,
        ITestOutputHelper testOutputHelper)
    {
        _scenarioContext = scenarioContext;
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    [Given(@"the following cars")]
    public void GivenTheFollowingCars(Table table)
    {
        var createCarCommands = table.CreateSet<CreateCarCommand>();
        _scenarioContext.Add("CreateCarCommands", createCarCommands);
    }

    [When(@"I create those cars")]
    public async Task WhenICreateThoseCars()
    {
        var createCarCommands = _scenarioContext.Get<List<CreateCarCommand>>("CreateCarCommands");
        foreach (var createCarCommand in createCarCommands)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Cars", createCarCommand);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }

    [Then(@"those cars get created successfully")]
    public async Task ThenThoseCarsGetCreatedSuccessfully()
    {
        var cars = _scenarioContext.Get<List<CreateCarCommand>>("CreateCarCommands");
        var response = await _httpClient.GetAsync("/api/Cars");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdCars = await response.Content.ReadFromJsonAsync<List<Car>>();
        foreach (var car in cars) createdCars.Should().ContainEquivalentOf(car);
    }

    [Given(@"the following cars in the system")]
    public async Task GivenTheFollowingCarsInTheSystem(Table table)
    {
        var carsToBeCreated = table.CreateSet<CreateCarCommand>();
        var createdCars = new List<Car>();
        foreach (var createCarCommand in carsToBeCreated)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Cars", createCarCommand);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.Conflict);
            var car = await response.Content.ReadFromJsonAsync<Car>();
            createdCars.Add(car);
        }

        _scenarioContext.Add("CarsForQuerying", createdCars);
    }

    [When(@"I query the system for my cars")]
    public async Task WhenIQueryTheSystemForMyCars()
    {
        var response = await _httpClient.GetAsync("/api/Cars");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cars = await response.Content.ReadFromJsonAsync<List<Car>>();
        _scenarioContext.Add("QueriedCars", cars);
    }

    [Then(@"my cars get returned successfully")]
    public void ThenMyCarsGetReturnedSuccessfully()
    {
        var expectedCars = _scenarioContext.Get<List<Car>>("CarsForQuerying");
        var actualCars = _scenarioContext.Get<List<Car>>("QueriedCars");
        foreach (var expectedCar in expectedCars) actualCars.Should().ContainEquivalentOf(expectedCar);
    }

    [Given(@"the following values to update")]
    public void GivenTheFollowingValuesToUpdate(Table table)
    {
        var updateCommands = table.CreateSet<UpdateCarCommand>();
        _scenarioContext.Add("UpdateCarCommands", updateCommands);
    }

    [When(@"I update those cars with those values")]
    public async Task WhenIUpdateThoseCarsWithThoseValues()
    {
        var updateCommands = _scenarioContext.Get<List<UpdateCarCommand>>("UpdateCarCommands");
        var updatedCars = new List<Car>();
        foreach (var updateCarCommand in updateCommands)
        {
            var response = await _httpClient
                .PutAsJsonAsync($"/api/Cars/{updateCarCommand.Id}", updateCarCommand);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var car = await response.Content.ReadFromJsonAsync<Car>();
            updatedCars.Add(car);
        }

        _scenarioContext.Add("UpdatedCars", updatedCars);
    }

    [Then(@"the cars get updated successfully")]
    public async Task ThenTheCarsGetUpdatedSuccessfully()
    {
        var updatedCars = _scenarioContext.Get<List<Car>>("UpdatedCars");

        var response = await _httpClient.GetAsync("api/Cars");
        var cars = await response.Content.ReadFromJsonAsync<List<Car>>();

        foreach (var updatedCar in updatedCars) cars.Should().ContainEquivalentOf(updatedCar);
    }

    [When(@"I delete those cars")]
    public async Task WhenIDeleteThoseCars()
    {
        var existingCars = _scenarioContext.Get<List<Car>>("CarsForQuerying");

        var statusCodes = new List<HttpStatusCode>();
        foreach (var existingCar in existingCars)
        {
            var response = await _httpClient.DeleteAsync($"api/Cars/{existingCar.Id}");
            statusCodes.Add(response.StatusCode);
        }

        _scenarioContext.Add("DeletedResponsesStatusCodes", statusCodes);
    }

    [Then(@"the cars get deleted successfully")]
    public async Task ThenTheCarsGetDeletedSuccessfully()
    {
        var givenCars = _scenarioContext.Get<List<Car>>("CarsForQuerying");
        var statusCodes = _scenarioContext.Get<List<HttpStatusCode>>("DeletedResponsesStatusCodes");
        statusCodes.Should().OnlyContain(x => x == HttpStatusCode.NoContent,
            "then the deletion was successful");

        var response = await _httpClient.GetAsync("api/Cars");
        var existingCars = await response.Content.ReadFromJsonAsync<List<Car>>();
        foreach (var givenCar in givenCars)
        {
            existingCars.Should().NotContainEquivalentOf(givenCar);
        }
    }
}