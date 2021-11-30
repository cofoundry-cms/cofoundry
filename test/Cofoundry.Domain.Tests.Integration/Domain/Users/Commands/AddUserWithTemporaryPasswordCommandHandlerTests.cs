using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.SeedData;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class AddUserWithTemporaryPasswordCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddUserWTempPWCHT-";
        const string EMAIL_DOMAIN = "@example.com";
        private readonly DbDependentTestApplicationFactory _appFactory;

        public AddUserWithTemporaryPasswordCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task MapsData()
        {
            // Ensure mapping to underlying AddUserCommand
            var uniqueData = UNIQUE_PREFIX + nameof(MapsData);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var command = new AddUserWithTemporaryPasswordCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                FirstName = "Leeroy",
                LastName = "Jenkins",
                RoleCode = app.SeededEntities.TestUserArea1.RoleA.RoleCode,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
            };

            await contentRepository
                .Users()
                .AddUserWithTemporaryPasswordAsync(command);

            var user = await dbContext
                .Users
                .AsNoTracking()
                .Include(r => r.Role)
                .FilterById(command.OutputUserId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputUserId.Should().BePositive();
                user.Should().NotBeNull();
                user.FirstName.Should().Be(command.FirstName);
                user.LastName.Should().Be(command.LastName);
                user.Email.Should().Be(command.Email);
                user.Password.Should().NotBeNullOrWhiteSpace();
                user.RequirePasswordChange.Should().BeTrue();
                user.Role.RoleCode.Should().Be(command.RoleCode);
                user.UserAreaCode.Should().Be(command.UserAreaCode);
                user.Username.Should().Be(user.Email);
            }
        }

        [Fact]
        public async Task WhenNotPasswordLogin_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotPasswordLogin_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutPasswordLogin.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserWithTemporaryPasswordCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*must*support*password*logins*");
        }

        [Fact]
        public async Task WhenNotEmailAsUsername_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotEmailAsUsername_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
            var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

            var command = new AddUserWithTemporaryPasswordCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                RoleId = roleId,
                UserAreaCode = userAreaCode
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*must*support*email*logins*");
        }

        [Fact]
        public async Task SendsMail()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);
            var password = "3110n3wm@n";
            using var app = _appFactory.Create(s =>
            {
                var mockPasswordGenerator = new Mock<IPasswordGenerationService>();
                mockPasswordGenerator.Setup(m => m.Generate()).Returns(password);
                s.AddSingleton(mockPasswordGenerator.Object);
            });
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();
            var loginUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.LoginPath);

            var command = new AddUserWithTemporaryPasswordCommand()
            {
                Email = uniqueData + EMAIL_DOMAIN,
                RoleCode = app.SeededEntities.TestUserArea1.RoleA.RoleCode,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
            };

            await contentRepository
                .Users()
                .AddUserWithTemporaryPasswordAsync(command);

            app.Mocks
                .CountDispatchedMail(
                    command.Email,
                    "account has been created",
                    "Test Site",
                    "username is: " + command.Email,
                    "password is: " + password,
                    loginUrl
                )
                .Should().Be(1);
        }
    }
}
