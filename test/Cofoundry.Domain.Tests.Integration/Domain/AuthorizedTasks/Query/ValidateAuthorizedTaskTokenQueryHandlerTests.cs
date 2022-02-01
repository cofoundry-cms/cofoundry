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

namespace Cofoundry.Domain.Tests.Integration.AuthorizedTasks.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class ValidateAuthorizedTaskTokenQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "ValAuthTskTQHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public ValidateAuthorizedTaskTokenQueryHandlerTests(
             DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task Valid_ReturnsValid()
        {
            var uniqueData = UNIQUE_PREFIX + "Valid";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var seedDate = new DateTime(2022, 02, 20);
            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate, c =>
                {
                    c.TaskData = uniqueData;
                    c.ExpireAfter = TimeSpan.FromHours(16);
                });

            app.Mocks.MockDateTime(seedDate.AddHours(10));
            var result = await contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                    Token = addAuthorizedTaskCommand.OutputToken
                })
                .ExecuteAsync();

            var task = await dbContext
                .AuthorizedTasks
                .SingleOrDefaultAsync(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.Error.Should().BeNull();
                result.Data.Should().NotBeNull();
                result.Data.AuthorizedTaskId.Should().Be(task.AuthorizedTaskId);
                result.Data.UserId.Should().Be(task.UserId);
                result.Data.TaskData.Should().Be(uniqueData);
                result.Data.UserAreaCode.Should().Be(TestUserArea1.Code);
            }
        }

        [Fact]
        public async Task WhenExpired_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WExpired_Err";
            var seedDate = new DateTime(2021, 02, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate, c =>
                {
                    c.ExpireAfter = TimeSpan.FromHours(16);
                });

            app.Mocks.MockDateTime(seedDate.AddHours(17));
            var result = await contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                    Token = addAuthorizedTaskCommand.OutputToken
                })
                .ExecuteAsync();

            AssertError(result, AuthorizedTaskValidationErrors.TokenValidation.Expired);
        }

        [Fact]
        public async Task WhenAlreadyComplete_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "Complete_Err";
            var seedDate = new DateTime(2019, 02, 20);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate);

            await contentRepository
                .AuthorizedTasks()
                .CompleteAsync(new CompleteAuthorizedTaskCommand()
                {
                    AuthorizedTaskId = addAuthorizedTaskCommand.OutputAuthorizedTaskId
                });

            var result = await contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                    Token = addAuthorizedTaskCommand.OutputToken,
                })
                .ExecuteAsync();

            AssertError(result, AuthorizedTaskValidationErrors.TokenValidation.AlreadyComplete);
        }

        [Fact]
        public async Task WhenInvalid_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WInv_Err";
            var seedDate = new DateTime(2018, 02, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate);

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
                .SingleAsync();
            authorizedTask.InvalidatedDate = seedDate;
            await dbContext.SaveChangesAsync();

            var result = await contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                    Token = addAuthorizedTaskCommand.OutputToken,
                })
                .ExecuteAsync();

            AssertError(result, AuthorizedTaskValidationErrors.TokenValidation.Invalidated);
        }

        [Fact]
        public async Task WhenNotExists_ReturnsError()
        {
            var uniqueData = UNIQUE_PREFIX + "WNotEx_Err";
            var seedDate = new DateTime(2017, 02, 20);

            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var addAuthorizedTaskCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate);

            var result = await contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                    Token = addAuthorizedTaskCommand.OutputToken + "X",
                })
                .ExecuteAsync();

            AssertError(result, AuthorizedTaskValidationErrors.TokenValidation.NotFound);
        }

        private static void AssertError(AuthorizedTaskTokenValidationResult result, ValidationErrorTemplate errorTemplate)
        {
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.Data.Should().BeNull();
                result.Error.Should().NotBeNull();
                result.Error.ErrorCode.Should().Be(errorTemplate.ErrorCode);
            }
        }
    }
}