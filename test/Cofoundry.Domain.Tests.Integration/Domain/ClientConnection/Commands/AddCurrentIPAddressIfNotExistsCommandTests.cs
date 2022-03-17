using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.ClientConnection.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddCurrentIPAddressIfNotExistsCommandTests
{
    const string UNIQUE_PREFIX = "AddCurrentIPAddressIfNotExistsCHT-";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public AddCurrentIPAddressIfNotExistsCommandTests(
         DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Theory]
    [InlineData("118.118.118.118")]
    [InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334")]
    [InlineData("2001:db8::8a2e:370:7334")]
    public async Task WhenNotExists_Adds(string address)
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        app.Mocks.MockIPAddress(address);
        var command = new AddCurrentIPAddressIfNotExistsCommand();
        await contentRepository.ExecuteCommandAsync(command);

        var dbIPAddress = await dbContext
            .IPAddresses
            .AsNoTracking()
            .Where(i => i.IPAddressId == command.OutputIPAddressId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            dbIPAddress.Should().NotBeNull();
            dbIPAddress.Address.Should().Be(address);
            dbIPAddress.CreateDate.Should().NotBeDefault();
        }
    }

    [Fact]
    public async Task WhenExists_ReturnsId()
    {
        const string address = "4.8.15.16";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepository();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        app.Mocks.MockIPAddress(address);
        var command1 = new AddCurrentIPAddressIfNotExistsCommand();
        await contentRepository.ExecuteCommandAsync(command1);
        var command2 = new AddCurrentIPAddressIfNotExistsCommand();
        await contentRepository.ExecuteCommandAsync(command2);

        command1.OutputIPAddressId.Should().BePositive().And.Be(command2.OutputIPAddressId);
    }
}
