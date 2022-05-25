using FluentValidation;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Create;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class CreateCarCommandValidatorTests
{
    private readonly IValidator<CreateCarCommand> _sut;

    public CreateCarCommandValidatorTests()
    {
        _sut = new CreateCarCommandValidator();
    }

    [Fact]
    public void Validate_ShouldBeTrue_WhenDetailsAreValid()
    {
        // Arrange
        const string name = "ABCEDFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü -_";
        var command = new CreateCarCommand
        {
            Name = name
        };

        // Act
        var result = _sut.Validate(command).IsValid;

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("§")]
    [InlineData("$")]
    [InlineData("%")]
    [InlineData("&")]
    [InlineData("/")]
    [InlineData("]")]
    [InlineData("@")]
    [InlineData("?")]
    public void Validate_ShouldBeFalse_WhenNameIsInvalid(string name)
    {
        // Arrange
        var command = new CreateCarCommand
        {
            Name = name
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }
}