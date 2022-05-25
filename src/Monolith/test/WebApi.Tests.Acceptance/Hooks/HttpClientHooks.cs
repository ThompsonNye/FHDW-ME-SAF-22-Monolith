using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks.Helpers;
using TechTalk.SpecFlow;
using Xunit.Abstractions;
using ConfigurationProvider = Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks.Helpers.ConfigurationProvider;

namespace Nuyken.Vegasco.Backend.WebApi.Tests.Acceptance.Hooks;

[Binding]
public sealed class HttpClientHooks
{
    private static JwtResponse _authorizationResponse;
    private readonly IObjectContainer _objectContainer;
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpClientHooks(IObjectContainer objectContainer, ITestOutputHelper testOutputHelper)
    {
        _objectContainer = objectContainer;
        _testOutputHelper = testOutputHelper;
    }

    [BeforeScenario]
    public async Task SetupHttpClient()
    {
        var apiBaseAddress = ConfigurationProvider.Configuration.GetValue<string>("ApiBaseAddress");
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseAddress)
        };
        await AddAuthorizationAsync(httpClient);
        _objectContainer.RegisterInstanceAs(httpClient);
    }

    private async Task AddAuthorizationAsync(HttpClient httpClient)
    {
        if (_authorizationResponse is not null)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_authorizationResponse!.TokenType,
                    _authorizationResponse!.AccessToken);
            return;
        }

        var config = ConfigurationProvider.Configuration;
        var authOptions = config
            .GetSection("ApiAuthorization")
            .Get<AuthorizationOptions>();

        using var authHttpClient = new HttpClient();
        var bodyContents = new KeyValuePair<string, string>[]
        {
            new("grant_type", "password"),
            new("username", authOptions.Username),
            new("password", authOptions.Password),
            new("client_id", authOptions.ClientId),
            new("client_secret", authOptions.ClientSecret)
        };
        using var content = new FormUrlEncodedContent(bodyContents);
        var response = await authHttpClient.PostAsync(authOptions.AuthenticationUrl, content);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            _testOutputHelper.WriteLine("Response code: {0}", response.StatusCode);
            _testOutputHelper.WriteLine("Content: {0}", await response.Content.ReadAsStringAsync());
            throw new AuthenticationException("Could not get Bearer");
        }

        _authorizationResponse = await response.Content.ReadFromJsonAsync<JwtResponse>();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(_authorizationResponse!.TokenType, _authorizationResponse!.AccessToken);
    }
}