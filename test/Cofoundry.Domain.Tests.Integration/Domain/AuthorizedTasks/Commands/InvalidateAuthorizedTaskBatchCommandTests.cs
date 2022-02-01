using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.AuthorizedTasks.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class InvalidateAuthorizedTaskBatchCommandTests
    {
        const string UNIQUE_PREFIX = "InvAuthTaskBCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public InvalidateAuthorizedTaskBatchCommandTests(
             DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task InvalidatesSameUser()
        {
            var uniqueData = UNIQUE_PREFIX + "InvSameUsr";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addAuthorizedTaskCommand = await app
                    .TestData
                    .AuthorizedTasks()
                    .AddWithNewUserAsync(uniqueData, null);

            var addAuthorizedTaskCommand2 = await app
                    .TestData
                    .AuthorizedTasks()
                    .AddWithNewUserAsync(uniqueData + "2", null);

            var command = await app
                    .TestData
                    .AuthorizedTasks()
                    .AddAsync(addAuthorizedTaskCommand.UserId, null);

            await contentRepository
                .AuthorizedTasks()
                .InvalidateBatchAsync(new InvalidateAuthorizedTaskBatchCommand(addAuthorizedTaskCommand.UserId));

            var authorizedTasks = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Where(t => t.UserId == addAuthorizedTaskCommand.UserId || t.UserId == addAuthorizedTaskCommand2.UserId)
                .ToListAsync();

            var invalidatedTasks = authorizedTasks.Where(t => t.UserId == addAuthorizedTaskCommand.UserId && t.InvalidatedDate.HasValue);
            var ignoredTask = authorizedTasks.Where(t => t.UserId == addAuthorizedTaskCommand2.UserId && !t.InvalidatedDate.HasValue);

            using (new AssertionScope())
            {
                authorizedTasks.Should().HaveCount(3);
                invalidatedTasks.Should().HaveCount(2);
                ignoredTask.Should().HaveCount(1);
            }
        }

        [Fact]
        public async Task CanFilterByTaskType()
        {
            var uniqueData = UNIQUE_PREFIX + "FilterTaskT";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, null, c => c.AuthorizedTaskTypeCode = UserAccountRecoveryAuthorizedTaskType.Code);

            await app
                .TestData
                .AuthorizedTasks()
                .AddAsync(addAuthorizedTaskCommand.UserId, null, c => c.AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code);

            await app
                .TestData
                .AuthorizedTasks()
                .AddAsync(addAuthorizedTaskCommand.UserId, null, c => c.AuthorizedTaskTypeCode = TestAuthorizedTaskType2.Code);

            await contentRepository
                .AuthorizedTasks()
                .InvalidateBatchAsync(new InvalidateAuthorizedTaskBatchCommand(addAuthorizedTaskCommand.UserId, TestAuthorizedTaskType1.Code, TestAuthorizedTaskType2.Code));

            var authorizedTasks = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Where(t => t.UserId == addAuthorizedTaskCommand.UserId)
                .ToListAsync();

            var invalidatedTasks = authorizedTasks.Where(t => 
                (t.AuthorizedTaskTypeCode == TestAuthorizedTaskType1.Code || t.AuthorizedTaskTypeCode == TestAuthorizedTaskType2.Code)
                && t.InvalidatedDate.HasValue
                );
            var ignoredTask = authorizedTasks.Where(t => t.AuthorizedTaskTypeCode == UserAccountRecoveryAuthorizedTaskType.Code && !t.InvalidatedDate.HasValue);

            using (new AssertionScope())
            {
                authorizedTasks.Should().HaveCount(3);
                invalidatedTasks.Should().HaveCount(2);
                ignoredTask.Should().HaveCount(1);
            }
        }
    }
}