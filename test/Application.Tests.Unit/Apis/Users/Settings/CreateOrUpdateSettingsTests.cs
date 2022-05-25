using System;
using Nuyken.VeGasCo.Backend.Application.Apis.Users.Settings.Upload;
using Nuyken.VeGasCo.Backend.Domain.Common.Enums;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Users.Settings;

public class CreateOrUpdateSettingsTests
{
    [Theory]
    [InlineData(false, null)]
    [InlineData(false, 3)]
    [InlineData(false, -1)]
    [InlineData(true, AppColorDesign.Bright)]
    [InlineData(true, AppColorDesign.Dark)]
    [InlineData(true, AppColorDesign.System)]
    public void TestValidation(bool expected, int? acd)
    {
        AppColorDesign? appColorDesign = acd.HasValue ? Enum.Parse<AppColorDesign>(acd.ToString()) : null;

        var command = new UploadSettingsCommand
        {
            AppColorDesign = appColorDesign
        };
        var validator = new UploadSettingsCommandValidator();

        var result = validator.Validate(command);

        Assert.Equal(expected, result.IsValid);
    }
}