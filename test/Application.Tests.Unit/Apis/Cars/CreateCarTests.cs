using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Cars;

public class CreateCarTests
{
    [Theory]
    [InlineData(false, "")]
    [InlineData(false, null)]
    // Test false names
    [InlineData(false, "😀")]
    [InlineData(false, "掲高野内球好月")]
    [InlineData(false, "後揚ヲミ代")]
    [InlineData(false, "Лорем ипсум")]
    [InlineData(false, "Λορεμ ιπσθμ")]
    [InlineData(false, "ლორემ იფსუმ")]
    [InlineData(false, "م الأوروبية أم,")]
    [InlineData(false, "----- ····· ·- --·· ")]
    [InlineData(false, "Test Name#")]
    [InlineData(false, "Test;Data")]
    [InlineData(false, "!")]
    [InlineData(false, "\"")]
    [InlineData(false, "§")]
    [InlineData(false, "$")]
    [InlineData(false, "%")]
    [InlineData(false, "&")]
    [InlineData(false, "/")]
    [InlineData(false, "(")]
    [InlineData(false, ")")]
    [InlineData(false, "=")]
    [InlineData(false, "?")]
    [InlineData(false, "*")]
    [InlineData(false, "'")]
    [InlineData(false, "#")]
    [InlineData(false, "°")]
    [InlineData(false, "^")]
    [InlineData(false, "{")]
    [InlineData(false, "]")]
    [InlineData(false, "`")]
    [InlineData(false, "²")]
    // Correct data
    [InlineData(true, "Test Name")]
    [InlineData(true, "Test_Data")]
    [InlineData(true, "Test-Data")]
    [InlineData(true, "Test Name 123")]
    [InlineData(true, "Cooles_ 123 - Autö")]
    [InlineData(true, "abcdefghijklmnopqrstuvwxyzäöüß_ -ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ0123456789")]
    public void TestValidation(bool expectedIsValid, string name)
    {
        var validator = new CreateCarCommandValidator();
        var command = new CreateCarCommand
        {
            Name = name
        };

        var result = validator.Validate(command);

        Assert.Equal(expectedIsValid, result.IsValid);
    }
}