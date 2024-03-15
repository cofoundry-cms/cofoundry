namespace Cofoundry.Domain.Tests.Integration.SeedData;

public class TestRoleInfo
{
    public required string RoleCode { get; set; }

    public int RoleId { get; set; }

    public TestUserInfo User { get; set; } = new();
}
