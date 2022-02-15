using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class LogSuccessfulAuthenticationCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "LogSAuthCHT-";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public LogSuccessfulAuthenticationCommandHandlerTests(
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

            var userId = await app.TestData.Users().AddAsync(uniqueData);
            await contentRepository.ExecuteCommandAsync(new LogSuccessfulAuthenticationCommand()
            {
                UserId = userId
            });

            var log = await dbContext
                .UserAuthenticationLogs
                .AsNoTracking()
                .Include(i => i.IPAddress)
                .SingleOrDefaultAsync(l => l.UserId == userId);

            using (new AssertionScope())
            {
                log.Should().NotBeNull();
                log.IPAddress.Address.Should().Be(uniqueData);
            }
        }
    }
}