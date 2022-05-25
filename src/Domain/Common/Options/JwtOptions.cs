namespace Nuyken.VeGasCo.Backend.Domain.Common.Options;

public class JwtOptions
{
    public string Audience { get; set; }

    public string Authority { get; set; }

    public string Issuer { get; set; }

    public bool EnableRoleBasedAuthorization { get; set; } = true;

    public string RoleKey { get; set; }

    public string RoleValue { get; set; }
}