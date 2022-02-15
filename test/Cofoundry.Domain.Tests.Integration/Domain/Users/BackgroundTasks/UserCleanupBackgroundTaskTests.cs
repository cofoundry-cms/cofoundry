using Cofoundry.Domain.BackgroundTasks;
using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Domain.Tests.Shared.Mocks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.BackgroundTasks
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UserCleanupBackgroundTaskTests
    {
        private readonly DbDependentTestApplicationFactory _appFactory;

        public UserCleanupBackgroundTaskTests(
             DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenEnabled_Runs()
        {
            CleanupUsersCommand executedCommand = null;
            using var app = _appFactory.Create(s =>
            {
                s.Configure<UsersSettings>(c =>
                {
                    c.Cleanup.DefaultRetentionPeriodInDays = 11;
                    c.Cleanup.AuthenticationLogRetentionPeriodInDays = 12;
                    c.Cleanup.AuthenticationFailLogRetentionPeriodInDays = 13;
                });
                s.MockHandler<CleanupUsersCommand>(c => executedCommand = c);
            });

            var backgroundTask = app.Services.GetRequiredService<UserCleanupBackgroundTask>();
            await backgroundTask.ExecuteAsync();

            using (new AssertionScope())
            {
                executedCommand.Should().NotBeNull();
                executedCommand.DefaultRetentionPeriod.Should().Be(TimeSpan.FromDays(11));
                executedCommand.AuthenticationLogRetentionPeriod.Should().Be(TimeSpan.FromDays(12));
                executedCommand.AuthenticationFailLogRetentionPeriod.Should().Be(TimeSpan.FromDays(13));
            }
        }

        [Fact]
        public async Task WhenDisabled_DoesNotRun()
        {
            CleanupUsersCommand executedCommand = null;
            using var app = _appFactory.Create(s =>
            {
                s.Configure<UsersSettings>(c => c.Cleanup.Enabled = false);
                s.MockHandler<CleanupUsersCommand>(c => executedCommand = c);
            });

            var backgroundTask = app.Services.GetRequiredService<UserCleanupBackgroundTask>();
            await backgroundTask.ExecuteAsync();

            using (new AssertionScope())
            {
                executedCommand.Should().BeNull();
            }
        }
    }
}