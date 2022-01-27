using Cofoundry.Domain.Tests.Shared.SeedData;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class ValidateUsernameQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "ValUsernameQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public ValidateUsernameQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
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

            var identitySettings = new UsersSettings();
            identitySettings.Username.MaxLength = 15;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
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

            var identitySettings = new UsersSettings();
            identitySettings.Username.MinLength = uniqueData.Length + 1;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
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

            AssertErrorMessage(result, "min-length-not-met", $"* less than {identitySettings.Username.MinLength} *");
        }

        [Fact]
        public async Task WhenInvalidFormat_ReturnsInvalid()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenInvalidFormat_ReturnsInvalid);

            var identitySettings = new UsersSettings();
            identitySettings.Username.AllowAnyCharacter = false;
            identitySettings.Username.AllowAnyLetter = false;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
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
                result.Error.ErrorCode.Should().Be("cf-user-username-" + codeSuffix);
                result.Error.Message.Should().Match(messagePattern);
            }
        }

    }
}
