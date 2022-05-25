using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Nuyken.VeGasCo.Backend.Domain.Entities.Common;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks;

public class DataCleanupHooks
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public DataCleanupHooks(HttpClient httpClient, ITestOutputHelper testOutputHelper, ScenarioContext scenarioContext)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
        _scenarioContext = scenarioContext;
    }

    [AfterScenario]
    public async Task CleanUpScenarioData()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Cars");
            var existingCars = await response.Content.ReadFromJsonAsync<List<Car>>();
            var allSuccessful = true;
            foreach (var existingCar in existingCars ?? Enumerable.Empty<Car>())
                try
                {
                    await _httpClient.DeleteAsync($"api/Cars/{existingCar.Id}");
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine("Error deleting car '{0}' with exception:\n{1}",
                        existingCar.Id, e);
                    allSuccessful = false;
                }

            if (allSuccessful)
                _testOutputHelper.WriteLine("Successfully cleaned up data for scenario '{0}'",
                    _scenarioContext.ScenarioInfo.Title);
        }
        catch (Exception e)
        {
            _testOutputHelper.WriteLine(e.ToString());
        }
    }
}