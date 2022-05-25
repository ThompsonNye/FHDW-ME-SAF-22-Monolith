namespace Nuyken.VeGasCo.Backend.Domain.Common.Options;

public class DatabaseOptions
{
    public DatabaseOptions()
    {
        AutoMigrate = false;
        Type = string.Empty;
    }

    public bool AutoMigrate { get; set; }

    public string Type { get; set; }
}