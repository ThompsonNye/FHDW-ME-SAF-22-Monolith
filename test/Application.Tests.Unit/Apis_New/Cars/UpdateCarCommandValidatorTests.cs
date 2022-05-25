using System;
using System.Collections.Generic;
using AutoFixture;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Nuyken.VeGasCo.Backend.Application.Apis.Cars.Update;
using Shouldly;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Apis_New.Cars;

public class UpdateCarCommandValidatorTests
{
    private readonly IActionContextAccessor _actionContextAccessor = Substitute.For<IActionContextAccessor>();
    private readonly IFixture _fixture = new Fixture();
    private readonly IValidator<UpdateCarCommand> _sut;


    public UpdateCarCommandValidatorTests()
    {
        _sut = new UpdateCarCommandValidator(_actionContextAccessor);
    }

    [Fact]
    public void Validate_ShouldBeTrue_WhenDetailsAreValid()
    {
        // Arrange
        const string name = "ABCEDFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü -_";
        var id = Guid.NewGuid();
        SetupActionContext(id);

        var command = new UpdateCarCommand
        {
            Id = id,
            Name = name
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    private void SetupActionContext(Guid requestId)
    {
        var values = new Dictionary<string, object> {{"id", requestId.ToString()}};
        var routeValues = new RouteValueDictionary(values);
        var routeData = new RouteData(routeValues);
        var actionContext = new ActionContext {RouteData = routeData};
        _actionContextAccessor.ActionContext.Returns(actionContext);
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
        var id = new Guid();
        SetupActionContext(id);
        var command = new UpdateCarCommand
        {
            Id = id,
            Name = name
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void Validate_ShouldBeFalse_WhenIdsMismatch()
    {
        // Arrange
        var id = new Guid();
        SetupActionContext(id);
        var command = _fixture.Build<UpdateCarCommand>()
            .With(x => x.Id, Guid.Empty)
            .Create();

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }
}