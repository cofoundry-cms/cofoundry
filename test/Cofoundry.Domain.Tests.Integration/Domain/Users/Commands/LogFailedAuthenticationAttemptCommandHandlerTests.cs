using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class LogFailedAuthenticationAttemptCommandHandlerTests
{
    const string UNIQUE_PREFIX = "LogFauthCHT-";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public LogFailedAuthenticationAttemptCommandHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CanLog()
    {
        var uniqueData = UNIQUE_PREFIX + "CanLog";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        app.Mocks.MockIPAddress(uniqueData);

        await contentRepository.ExecuteCommandAsync(new LogFailedAuthenticationAttemptCommand(TestUserArea1.Code, uniqueData));

        var log = await dbContext
            .UserAuthenticationFailLogs
            .AsNoTracking()
            .Include(i => i.IPAddress)
            .SingleOrDefaultAsync(l => l.Username == uniqueData);

        using (new AssertionScope())
        {
            log.Should().NotBeNull();
            log.IPAddress.Address.Should().Be(uniqueData);

            app.Mocks
                .CountMessagesPublished<UserAuthenticationFailedMessage>(m =>
                {
                    return m.UserAreaCode == TestUserArea1.Code && m.Username == uniqueData;
                })
                .Should().Be(1);
        }
    }
}
