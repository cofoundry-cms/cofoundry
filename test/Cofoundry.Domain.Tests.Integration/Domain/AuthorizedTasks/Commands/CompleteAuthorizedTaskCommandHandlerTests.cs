using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
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
    public class CompleteAuthorizedTaskCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "CompAuthTaskCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public CompleteAuthorizedTaskCommandHandlerTests(
             DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanComplete()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanComplete);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addAuthorizedTaskCommand = await app
                    .TestData
                    .AuthorizedTasks()
                    .AddWithNewUserAsync(uniqueData, null);

            await contentRepository
                .AuthorizedTasks()
                .CompleteAsync(new CompleteAuthorizedTaskCommand(addAuthorizedTaskCommand.OutputAuthorizedTaskId));

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
                .SingleAsync();

            using (new AssertionScope())
            {
                authorizedTask.Should().NotBeNull();
                authorizedTask.CompletedDate.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task WhenInvalid_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "WInvalid";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var addAuthorizedTaskCommand = await app
                    .TestData
                    .AuthorizedTasks()
                    .AddWithNewUserAsync(uniqueData, null);

            await contentRepository
                .AuthorizedTasks()
                .InvalidateBatchAsync(new InvalidateAuthorizedTaskBatchCommand(addAuthorizedTaskCommand.UserId));

            await contentRepository
                    .Awaiting(r => r.AuthorizedTasks().CompleteAsync(new CompleteAuthorizedTaskCommand(addAuthorizedTaskCommand.OutputAuthorizedTaskId)))
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("* marked as invalid.");
        }
    }
}