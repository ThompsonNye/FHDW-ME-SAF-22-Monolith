namespace Nuyken.VeGasCo.Backend.Application.Apis.Info.User;

/// <summary>
///     Holds information about a user of the server, retrieved from the authentication mechanism
/// </summary>
public class GetUserInfoResponse
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Name { get; set; }
}