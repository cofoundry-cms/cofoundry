using Cofoundry.Domain.Tests.Shared;
using NSubstitute;

namespace Cofoundry.Domain.Tests.Integration.Users.Helpers;

/// <summary>
/// This is mostly covered by command tests, so we only need to test
/// custom implementation resolution here.
/// </summary>
[Collection(nameof(DbDependentFixtureCollection))]
public class UserDataFormatterTests
{
    private readonly DbDependentTestApplicationFactory _appFactory;

    public UserDataFormatterTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public void CanUseCustomUniquifier()
    {
        const string OUTPUT = "Tada! Ha ha ha...";

        var uniquifierMock = Substitute.For<IUsernameUniquifier<CofoundryAdminUserArea>>();
        uniquifierMock.Uniquify(Arg.Any<string>()).Returns(OUTPUT);

        using var app = _appFactory.Create(s => s.AddSingleton(uniquifierMock));
        var formatter = app.Services.GetRequiredService<IUserDataFormatter>();

        var cofoundryAdminResult = formatter.UniquifyUsername(CofoundryAdminUserArea.Code, "George Sanderson");
        var defaultResult = formatter.UniquifyUsername(TestUserArea1.Code, "Roz");

        cofoundryAdminResult.Should().Be(OUTPUT);
        defaultResult.Should().Be("roz");
    }
}
