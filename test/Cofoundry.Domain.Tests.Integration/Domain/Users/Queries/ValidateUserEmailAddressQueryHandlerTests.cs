using Cofoundry.Domain.Tests.Shared.SeedData;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class ValidateUserEmailAddressQueryHandlerTests
{
    const string UNIQUE_DOMAIN = "@ValUserEmailQHT.com";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public ValidateUserEmailAddressQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task Valid_ReturnsValid()
    {
        var uniqueData = nameof(Valid_ReturnsValid) + UNIQUE_DOMAIN;

        using var app = _appFactory.Create();

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateUsername(new ValidateUsernameQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
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
    public async Task WhenNoAtSymbol_ReturnsInvalidFormat()
    {
        var uniqueData = nameof(WhenNoAtSymbol_ReturnsInvalidFormat);

        using var app = _appFactory.Create();

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Email = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "invalid-format", "* invalid format.");
    }

    [Fact]
    public async Task WhenTooLong_ReturnsInvalid()
    {
        var uniqueData = nameof(WhenTooLong_ReturnsInvalid) + UNIQUE_DOMAIN;

        var userSettings = new UsersSettings();
        userSettings.EmailAddress.MaxLength = 15;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Email = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "max-length-exceeded", "* more than 15 *");
    }

    [Fact]
    public async Task WhenTooShort_ReturnsInvalid()
    {
        var uniqueData = "2S" + UNIQUE_DOMAIN;

        var userSettings = new UsersSettings();
        userSettings.EmailAddress.MinLength = uniqueData.Length + 1;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Email = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "min-length-not-met", $"* less than {userSettings.EmailAddress.MinLength} *");
    }

    [Fact]
    public async Task WhenInvalidFormat_ReturnsInvalid()
    {
        var uniqueData = nameof(WhenInvalidFormat_ReturnsInvalid) + UNIQUE_DOMAIN;

        var userSettings = new UsersSettings();
        userSettings.EmailAddress.AllowAnyCharacter = false;
        userSettings.EmailAddress.AllowAnyLetter = false;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Email = uniqueData
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "invalid-characters", "* cannot contain 'W'.");
    }

    [Fact]
    public async Task WhenNotUnique_ReturnsInvalid()
    {
        using var app = _appFactory.Create();

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = CofoundryAdminUserArea.Code,
                Email = app.SeededEntities.AdminUser.Username.ToUpperInvariant()
            })
            .ExecuteAsync();

        AssertErrorMessage(result, "not-unique", "* already registered.");
    }

    [Fact]
    public async Task WhenNotRequiredUnique_DoesNotValidateUnique()
    {
        var uniqueData = "NotUniq_NValUnq" + UNIQUE_DOMAIN;

        var userSettings = new UsersSettings();
        userSettings.EmailAddress.RequireUnique = false;
        using var app = _appFactory.Create(s =>
        {
            s.AddSingleton(userSettings);
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, UserAreaWithoutEmailAsUsername.Code);

        await contentRepository
            .Users()
            .AddAsync(new AddUserCommand()
            {
                Email = uniqueData,
                Username = uniqueData,
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                RoleId = roleId,
                Password = nameof(ValidateUserEmailAddressQueryHandlerTests)
            });

        var result = await contentRepository
            .Users()
            .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
            {
                UserAreaCode = UserAreaWithoutEmailAsUsername.Code,
                Email = uniqueData
            })
            .ExecuteAsync();

        result.IsSuccess.Should().BeTrue();
    }

    private static void AssertErrorMessage(ValidationQueryResult result, string codeSuffix, string messagePattern)
    {
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.ErrorCode.Should().Be("cf-user-email-" + codeSuffix);
            result.Error.Message.Should().Match(messagePattern);
        }
    }

}
