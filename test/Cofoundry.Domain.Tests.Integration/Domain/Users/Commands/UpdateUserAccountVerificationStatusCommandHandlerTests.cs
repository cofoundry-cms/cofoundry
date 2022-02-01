using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdateUserAccountVerificationStatusCommandHandlerTests
    {
        private const string UNIQUE_PREFIX = "UpdUsrVerStatusCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdateUserAccountVerificationStatusCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanSetVerified()
        {
            var uniqueData = UNIQUE_PREFIX + "CSetVer";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userId = await app.TestData.Users().AddAsync(uniqueData);

            await contentRepository
                .Users()
                .AccountVerification()
                .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
                {
                    IsAccountVerified = true,
                    UserId = userId
                });

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleAsync();

            using (new AssertionScope())
            {
                user.AccountVerifiedDate.Should().NotBeNull().And.NotBeDefault();

                app.Mocks
                    .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>(m => m.UserId == userId && m.UserAreaCode == user.UserAreaCode && m.IsVerified)
                    .Should().Be(1);
            }
        }

        [Fact]
        public async Task CanUnset()
        {
            var uniqueData = UNIQUE_PREFIX + "CUnset";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var userId = await app.TestData.Users().AddAsync(uniqueData);

            await contentRepository
                .Users()
                .AccountVerification()
                .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
                {
                    IsAccountVerified = true,
                    UserId = userId
                });

            await contentRepository
                .Users()
                .AccountVerification()
                .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
                {
                    IsAccountVerified = false,
                    UserId = userId
                });

            var user = await dbContext
                .Users
                .AsNoTracking()
                .FilterById(userId)
                .SingleAsync();

            using (new AssertionScope())
            {
                user.AccountVerifiedDate.Should().BeNull();

                app.Mocks
                    .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>(m => m.UserId == userId && m.UserAreaCode == user.UserAreaCode && !m.IsVerified)
                    .Should().Be(1);
            }
        }


        [Fact]
        public async Task IfNoChange_NoMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "NoChgNoMsg";

            int userId = 0;

            using (var app = _appFactory.Create())
            {
                var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

                userId = await app.TestData.Users().AddAsync(uniqueData);

                await contentRepository
                    .Users()
                    .AccountVerification()
                    .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
                    {
                        IsAccountVerified = true,
                        UserId = userId
                    });
            }

            using (var app = _appFactory.Create())
            {
                var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
                var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

                await contentRepository
                    .Users()
                    .AccountVerification()
                    .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
                    {
                        IsAccountVerified = true,
                        UserId = userId
                    });

                using (new AssertionScope())
                {
                    app.Mocks
                        .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>(m => m.UserId == userId)
                        .Should().Be(0);
                }
            }
        }
    }
}
