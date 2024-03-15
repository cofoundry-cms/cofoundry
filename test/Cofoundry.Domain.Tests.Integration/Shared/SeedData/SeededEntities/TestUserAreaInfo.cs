namespace Cofoundry.Domain.Tests.Integration.SeedData;

public class TestUserAreaInfo
{
    public required string UserAreaCode { get; set; }

    public required IUserAreaDefinition Definition { get; set; }

    public required TestRoleInfo RoleA { get; set; }

    public required TestRoleInfo RoleB { get; set; }

}
