using Cofoundry.Domain.Tests.Integration.SeedData;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// References to pre-seeded static/global test data. These can be used for convenience i
    /// n multiple tests as references but should not be altered.
    /// </summary>
    public class SeededEntities
    {
        /// <summary>
        /// A test generic template with one region named "Body" that accepts 
        /// multiple blocks of any type.
        /// </summary>
        public TestPageTemplateInfo TestPageTemplate { get; } = new TestPageTemplateInfo();

        /// <summary>
        /// A test custom entity template for custom entity 
        /// "TestCustomEntityDefinition" with a page region named "Page Body" and
        /// a custom entity region named "Custom Entity Body". Both regions
        /// accepts multiple blocks of any type.
        /// </summary>
        public TestCustomEntityPageTemplateInfo TestCustomEntityPageTemplate { get; } = new TestCustomEntityPageTemplateInfo();

        /// <summary>
        /// 80x80 image
        /// </summary>
        public int TestImageId { get; set; }

        /// <summary>
        /// Id of the root directory ("/").
        /// </summary>
        public int RootDirectoryId { get; set; }

        /// <summary>
        /// An existing tag which can be used in combination with a unique tag to test new and existing tag references.
        /// </summary>
        public TestTagInfo TestTag { get; } = new TestTagInfo();

        /// <summary>
        /// Directory with path "/test-directory".
        /// </summary>
        public TestDirectoryInfo TestDirectory { get; } = new TestDirectoryInfo();

        /// <summary>
        /// The Cofoundry admin user account initialized in the site setup.
        /// </summary>
        public TestUserInfo AdminUser { get; } = new TestUserInfo() { Username = "admin@example.com", Password = "x92Ro01kEpgA" };

        public TestCustomEntityDefinition TestCustomEntityDefinition { get; } = new TestCustomEntityDefinition();

        /// <summary>
        /// A basic test custom entity for the <see cref="TestCustomEntityDefinition"/>
        /// type.
        /// </summary>
        public TestCustomEntityInfo TestCustomEntity { get; set; } = new TestCustomEntityInfo();

        /// <summary>
        /// A test user area with a single role.
        /// </summary>
        public TestUserAreaInfo TestUserArea1 { get; set; } = new TestUserAreaInfo() { UserAreaCode = Shared.TestUserArea1.Code, RoleCode = Shared.TestUserArea1Role.Code };

        /// <summary>
        /// A secondary test user area with a single role.
        /// </summary>
        public TestUserAreaInfo TestUserArea2 { get; set; } = new TestUserAreaInfo() { UserAreaCode = Shared.TestUserArea2.Code, RoleCode = Shared.TestUserArea2Role.Code };
    }
}
