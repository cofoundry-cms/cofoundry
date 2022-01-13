using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;


namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class ResetUserPasswordCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "ResetUserPWCHT ";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public ResetUserPasswordCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ResetsPassword()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ResetsPassword);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userId = await app.TestData.Users().AddAsync(uniqueData);

            var originalUserState = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleAsync();

            await contentRepository
                .Users()
                .ResetPasswordAsync(userId);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                user.Should().NotBeNull();
                originalUserState.Password.Should().NotBeNull();
                user.Password.Should().NotBe(originalUserState.Password);
                user.LastPasswordChangeDate.Should().BeAfter(originalUserState.LastPasswordChangeDate);
                user.SecurityStamp.Should().NotBeNull().And.NotBe(originalUserState.SecurityStamp);
            }
        }

        [Fact]
        public async Task SendsMail()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);
            var password1 = "3110j3rr13";
            var password2 = "3110n3wm@n";

            using var app = _appFactory.Create(s =>
            {
                var mockPasswordGenerator = new Mock<IPasswordGenerationService>();
                mockPasswordGenerator
                    .SetupSequence(m => m.Generate(It.IsAny<int>()))
                    .Returns(password1)
                    .Returns(password2);
                s.AddSingleton(mockPasswordGenerator.Object);
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userId = await app.TestData.Users().AddAsync(uniqueData);
            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();
            var loginUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.LoginPath);

            await contentRepository
                .Users()
                .ResetPasswordAsync(userId);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            app.Mocks
                .CountDispatchedMail(
                    user.Email,
                    "Test Site",
                    "site administrator has reset your password",
                    "username is: " + user.Username,
                    "password is: " + password2,
                    loginUrl
                )
                .Should().Be(1);
        }

        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var userId = await app.TestData.Users().AddAsync(uniqueData);

            await contentRepository
                .Users()
                .ResetPasswordAsync(userId);

            app.Mocks
                .CountMessagesPublished<UserPasswordResetMessage>(m =>
                {
                    return m.UserId == userId && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m =>
                {
                    return m.UserId == userId
                        && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);
        }
    }
}
