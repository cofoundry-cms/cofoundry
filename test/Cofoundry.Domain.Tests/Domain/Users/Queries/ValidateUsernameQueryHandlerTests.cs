﻿using Cofoundry.Domain.Tests.Shared.SeedData;

namespace Cofoundry.Domain.Tests.Users.Queries;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class ValidateUsernameQueryHandlerTests
{
    const string UNIQUE_PREFIX = "ValUsernameQHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public ValidateUsernameQueryHandlerTests(
        IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task Valid_ReturnsValid()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(Valid_ReturnsValid);

        using var app = _appFactory.Create();

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Username = uniqueData
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenTooLong_ReturnsInvalid()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenTooLong_ReturnsInvalid);

        var userSettings = new UsersSettings();
        userSettings.Username.MaxLength = 15;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Username = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "max-length-exceeded", "* more than 15 *");
    }

    [Fact]
    public async Task WhenTooShort_ReturnsInvalid()
    {
        var uniqueData = UNIQUE_PREFIX + "2S";

        var userSettings = new UsersSettings();
        userSettings.Username.MinLength = uniqueData.Length + 1;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Username = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "min-length-not-met", $"* less than {userSettings.Username.MinLength} *");
    }

    [Fact]
    public async Task WhenInvalidFormat_ReturnsInvalid()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenInvalidFormat_ReturnsInvalid);

        var userSettings = new UsersSettings();
        userSettings.Username.AllowAnyCharacter = false;
        userSettings.Username.AllowAnyLetter = false;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Username = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "invalid-characters", "* cannot contain 'V'.");
    }

    [Fact]
    public async Task WhenNotUnique_ReturnsInvalid()
    {
        using var app = _appFactory.Create();

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Username = app.SeededEntities.AdminUser.Username.ToUpperInvariant()
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "not-unique", "* already registered.");
    }

    private static void AssertErrorMessage(ValidationQueryResult result, string codeSuffix, string messagePattern)
    {
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error?.ErrorCode.Should().Be("cf-user-username-" + codeSuffix);
            result.Error?.Message.Should().Match(messagePattern);
        }
    }

}
