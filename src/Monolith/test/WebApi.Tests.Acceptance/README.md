# WebApi Acceptance Tests

## Configuration

Some configuration needs to be done before the tests can be run. Add the `Authentication`-Section to
the `appsettings.json` file (the project's `secrets.json` file should work too) like shown below:

```json
{
  "DockerComposeFileName": "docker-compose.yml",
  "ApiBaseAddress": "http://localhost:8080",
  "ApiAuthorization": {
    "AuthenticationUrl": "https://auth.example.com/token-url",
    "ClientId": "client_id",
    "ClientSecret": "client_secret",
    "Username": "username",
    "Password": "password"
  }
}
```

The authentication here uses the Resource Owner Password Flow since the user cannot login himself / herself using the
usual Authentication Code Flow.

### Potential error

If these authorization details are not specified, the tests most likely show NullRef exceptions because the
authorization options cannot be extracted and thus the HttpClient cannot be added to the test's DI container. When the
test does not fail then, SpecFlow tries to inject the HttpClient into the step class but does not find a registered
instance in the DI container, tries to automatically resolve the dependency but cannot create an instance of the
abstract HttpMessageHandler class.

If any of those errors are shown in the test output, look in the `HttpClientHooks.cs` class first and check if all
needed configuration details are specified.
