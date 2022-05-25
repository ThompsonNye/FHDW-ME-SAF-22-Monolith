namespace Nuyken.VeGasCo.Backend.WebApi.Common.Models;

public class ErrorResponse
{
    public bool IsError => true;

    public string Message { get; set; }

    public object Error { get; set; }
}