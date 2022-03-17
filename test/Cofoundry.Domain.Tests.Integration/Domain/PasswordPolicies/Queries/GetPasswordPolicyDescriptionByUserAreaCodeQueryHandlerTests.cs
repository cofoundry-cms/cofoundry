namespace Cofoundry.Domain.Tests.Integration.UserAreas.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetPasswordPolicyDescriptionByUserAreaCodeQueryHandlerTests
{
    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetPasswordPolicyDescriptionByUserAreaCodeQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task ReturnsAndMaps()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var policyDescription = await contentRepository
            .UserAreas()
            .PasswordPolicies()
            .GetByCode(CofoundryAdminUserArea.Code)
            .AsDescription()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            policyDescription.Should().NotBeNull();
            policyDescription.Description.Should()
                .NotBeNull()
                .And
                .Match("*between 10 and 300*");

            policyDescription.Attributes.Should().NotBeNullOrEmpty();
            policyDescription.Attributes.Should().Contain(PasswordPolicyAttributes.MinLength, "10");
            policyDescription.Attributes.Should().Contain(PasswordPolicyAttributes.MaxLength, "300");

            policyDescription.Criteria.Should().NotBeNullOrEmpty();
            policyDescription.Criteria.Should()
                .ContainMatch("*at least 10*")
                .And
                .ContainMatch("*not be * current password*")
                ;
        }
    }
}
