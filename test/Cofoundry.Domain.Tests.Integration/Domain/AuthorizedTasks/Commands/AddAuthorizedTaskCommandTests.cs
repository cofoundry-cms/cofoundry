using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.AuthorizedTasks.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddAuthorizedTaskCommandTests
{
    const string UNIQUE_PREFIX = "AddAuthTskTCHT-";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public AddAuthorizedTaskCommandTests(
         DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CreatesTask()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CreatesTask);
        var seedDate = new DateTime(2018, 03, 20);

        using var app = _appFactory.Create();

        var addAuthorizedTaskCommand = await app
            .TestData
            .AuthorizedTasks()
            .AddWithNewUserAsync(uniqueData, seedDate, c => c.TaskData = uniqueData);
        var dbTask = await GetAuthorizedTaskAsync(addAuthorizedTaskCommand);

        var tokenFormatter = new AuthorizedTaskTokenFormatter();
        var token = tokenFormatter.Format(new AuthorizedTaskTokenParts()
        {
            AuthorizationCode = dbTask.AuthorizationCode,
            AuthorizedTaskId = dbTask.AuthorizedTaskId
        });

        using (new AssertionScope())
        {
            addAuthorizedTaskCommand.OutputToken.Should().Be(token);
            dbTask.Should().NotBeNull();
            dbTask.CreateDate.Should().NotBeDefault();
            dbTask.IPAddress.Address.Should().Be(TestIPAddresses.Localhost);
            dbTask.CompletedDate.Should().BeNull();
            dbTask.InvalidatedDate.Should().BeNull();
            dbTask.AuthorizationCode.Should().NotBeEmpty();
            dbTask.UserId.Should().BePositive();
            dbTask.AuthorizedTaskTypeCode.Should().Be(TestAuthorizedTaskType1.Code);
            dbTask.AuthorizedTaskId.Should().NotBeEmpty();
            dbTask.TaskData.Should().Be(uniqueData);
        }
    }

    [Fact]
    public async Task RateLimitExceeded_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + "RLimitExceeded";
        var seedDate = new DateTime(2022, 03, 19);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        app.Mocks.MockDateTime(seedDate);

        var initialCommand = await app
                .TestData
                .AuthorizedTasks()
                .AddWithNewUserAsync(uniqueData, seedDate);

        app.Mocks.MockDateTime(seedDate.AddHours(1));
        await contentRepository.AuthorizedTasks().AddAsync(CreateCommand());

        using (new AssertionScope())
        {
            app.Mocks.MockDateTime(seedDate.AddHours(1).AddMinutes(59));
            await contentRepository
                .Awaiting(r => r.AuthorizedTasks().AddAsync(CreateCommand()))
                .Should()
                .ThrowAsync<ValidationErrorException>()
                .WithErrorCode(AuthorizedTaskValidationErrors.Create.RateLimitExceeded.ErrorCode);

            app.Mocks.MockDateTime(seedDate.AddHours(2));
            await contentRepository
                .Awaiting(r => r.AuthorizedTasks().AddAsync(CreateCommand()))
                .Should()
                .NotThrowAsync();
        }

        AddAuthorizedTaskCommand CreateCommand()
        {
            return app
                .TestData
                .AuthorizedTasks()
                .CreateAddCommand(initialCommand.UserId, c =>
                {
                    c.RateLimit.Quantity = 2;
                    c.RateLimit.Window = TimeSpan.FromHours(2);
                });
        }
    }

    private async Task<AuthorizedTask> GetAuthorizedTaskAsync(AddAuthorizedTaskCommand addAuthorizedTaskCommand)
    {
        using var app = _appFactory.Create();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .Include(t => t.IPAddress)
            .Where(t => t.AuthorizedTaskId == addAuthorizedTaskCommand.OutputAuthorizedTaskId)
            .SingleAsync();

        return authorizedTask;
    }
}
