using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Queries
{
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
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Username = uniqueData
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsValid.Should().BeTrue();
                result.Errors.Should().BeEmpty();
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
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Email = uniqueData
                })
                .ExecuteAsync();

            AssertErrorMessage(result, "invalid-format", "* invalid format.");
        }

        [Fact]
        public async Task WhenTooLong_ReturnsInvalid()
        {
            var uniqueData = nameof(WhenTooLong_ReturnsInvalid) + UNIQUE_DOMAIN;

            var identitySettings = new IdentitySettings();
            identitySettings.EmailAddress.MaxLength = 15;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var result = await contentRepository
                .Users()
                .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Email = uniqueData
                })
                .ExecuteAsync();

            AssertErrorMessage(result, "max-length-exceeded", "* more than 15 *");
        }

        [Fact]
        public async Task WhenTooShort_ReturnsInvalid()
        {
            var uniqueData = "2S" + UNIQUE_DOMAIN;

            var identitySettings = new IdentitySettings();
            identitySettings.EmailAddress.MinLength = uniqueData.Length + 1;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var result = await contentRepository
                .Users()
                .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Email = uniqueData
                })
                .ExecuteAsync();

            AssertErrorMessage(result, "min-length-not-met", $"* less than {identitySettings.EmailAddress.MinLength} *");
        }

        [Fact]
        public async Task WhenInvalidFormat_ReturnsInvalid()
        {
            var uniqueData = nameof(WhenInvalidFormat_ReturnsInvalid) + UNIQUE_DOMAIN;

            var identitySettings = new IdentitySettings();
            identitySettings.EmailAddress.AllowAnyCharacter = false;
            identitySettings.EmailAddress.AllowAnyLetter = false;
            using var app = _appFactory.Create(s =>
            {
                s.AddSingleton(identitySettings);
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var result = await contentRepository
                .Users()
                .ValidateEmailAddress(new ValidateUserEmailAddressQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
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
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Email = app.SeededEntities.AdminUser.Username.ToUpperInvariant()
                })
                .ExecuteAsync();

            AssertErrorMessage(result, "not-unique", "* already registered.");
        }

        private static void AssertErrorMessage(ValidationQueryResult result, string codeSuffix, string messagePattern)
        {
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsValid.Should().BeFalse();
                result.Errors.Should().HaveCount(1);
                var error = result.Errors.Single();
                error.ErrorCode.Should().Be("cf-user-email-" + codeSuffix);
                error.Message.Should().Match(messagePattern);
            }
        }

    }
}
