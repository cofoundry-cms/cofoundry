using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;


namespace Cofoundry.Domain.Tests.Integration.Users.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class CompleteUserAccountVerificationByEmailCommandHandlerTests
{
    const string UNIQUE_PREFIX = "CompAccVerCHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public CompleteUserAccountVerificationByEmailCommandHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CompletesReset()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CompletesReset);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var authorizedTask = await AddUserAndInitiate(uniqueData, app);
        var token = MakeToken(authorizedTask);

        await contentRepository
            .Users()
            .AccountVerification()
            .EmailFlow()
            .CompleteAsync(new CompleteUserAccountVerificationViaEmailCommand()
            {
                Token = token,
                UserAreaCode = authorizedTask.User.UserAreaCode
            });

        var completedTask = await GetAuthorizedTask(app, authorizedTask.UserId);

        using (new AssertionScope())
        {
            completedTask.Should().NotBeNull();
            completedTask.CompletedDate.Should().NotBeNull();
            completedTask.User.AccountVerifiedDate.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task InvalidatesOldResetTokens()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CompletesReset);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var authenticatedTask1 = await AddUserAndInitiate(uniqueData + 1, app);
        var token = MakeToken(authenticatedTask1);
        var authenticatedTask2 = await AddUserAndInitiate(uniqueData + 2, app);

        await contentRepository
            .Users()
            .AccountVerification()
            .EmailFlow()
            .InitiateAsync(new InitiateUserAccountVerificationViaEmailCommand()
            {
                UserId = authenticatedTask1.UserId
            });

        await contentRepository
            .Users()
            .AccountVerification()
            .EmailFlow()
            .CompleteAsync(new CompleteUserAccountVerificationViaEmailCommand()
            {
                Token = token,
                UserAreaCode = authenticatedTask1.User.UserAreaCode,
            });

        var authenticatedTasks = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Where(u => u.UserId == authenticatedTask1.UserId || u.UserId == authenticatedTask2.UserId)
            .ToListAsync();

        var resetRequest1Result = authenticatedTasks.SingleOrDefault(r => r.AuthorizedTaskId == authenticatedTask1.AuthorizedTaskId);
        var resetRequest2Result = authenticatedTasks.SingleOrDefault(r => r.AuthorizedTaskId == authenticatedTask2.AuthorizedTaskId);
        var resetRequest3Result = authenticatedTasks.SingleOrDefault(r => r.UserId == authenticatedTask1.UserId && r.AuthorizedTaskId != authenticatedTask1.AuthorizedTaskId);

        using (new AssertionScope())
        {
            authenticatedTasks.Should().HaveCount(3);
            resetRequest1Result.Should().NotBeNull();
            resetRequest1Result.CompletedDate.Should().NotBeNull();
            resetRequest1Result.InvalidatedDate.Should().BeNull();

            resetRequest2Result.Should().NotBeNull();
            resetRequest2Result.CompletedDate.Should().BeNull();
            resetRequest2Result.InvalidatedDate.Should().BeNull();

            resetRequest3Result.Should().NotBeNull();
            resetRequest3Result.CompletedDate.Should().BeNull();
            resetRequest3Result.InvalidatedDate.Should().NotBeNull();
        }
    }

    private static string MakeToken(AuthorizedTask authorizedTask)
    {
        var formatter = new AuthorizedTaskTokenFormatter();
        return formatter.Format(new AuthorizedTaskTokenParts()
        {
            AuthorizationCode = authorizedTask.AuthorizationCode,
            AuthorizedTaskId = authorizedTask.AuthorizedTaskId
        });
    }

    private static async Task<AuthorizedTask> AddUserAndInitiate(
        string uniqueData,
        DbDependentTestApplication app,
        Action<AddUserCommand> configration = null
        )
    {
        var contentRepository = app.Services.GetContentRepository();
        var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        if (configration != null) configration(addUserCommand);

        var userId = await contentRepository
            .WithElevatedPermissions()
            .Users()
            .AddAsync(addUserCommand);

        await contentRepository
            .Users()
            .AccountVerification()
            .EmailFlow()
            .InitiateAsync(new InitiateUserAccountVerificationViaEmailCommand()
            {
                UserId = userId
            });

        var authorizedTask = await GetAuthorizedTask(app, userId);

        return authorizedTask;
    }

    private static async Task<AuthorizedTask> GetAuthorizedTask(
        DbDependentTestApplication app,
        int userId
        )
    {
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var authorizedTask = await dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Include(u => u.User)
            .SingleAsync(u => u.UserId == userId);

        return authorizedTask;
    }
}
