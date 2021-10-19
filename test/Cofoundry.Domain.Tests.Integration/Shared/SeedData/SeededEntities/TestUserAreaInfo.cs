namespace Cofoundry.Domain.Tests.Integration.SeedData
{
    public class TestUserAreaInfo
    {
        public string UserAreaCode { get; set; }
        
        public string RoleCode { get; set; }

        public IUserAreaDefinition Definition { get; set; }

        public int RoleId { get; set; }

        public TestUserInfo User { get; set; }

    }
}
