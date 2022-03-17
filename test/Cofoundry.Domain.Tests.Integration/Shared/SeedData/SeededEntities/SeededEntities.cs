using Cofoundry.Domain.Tests.Integration.SeedData;

namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// References to pre-seeded static/global test data. These can be used for convenience 
/// in multiple tests as references but should not be altered.
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

    /// <summary>
    /// A basic test custom entity for the <see cref="TestCustomEntityDefinition"/>
    /// type.
    /// </summary>
    public TestCustomEntityInfo TestCustomEntity { get; set; } = new TestCustomEntityInfo()
    {
        CustomEntityDefinitionCode = TestCustomEntityDefinition.Code,
        UrlSlug = "test-custom-entity",
        Title = "Test Custom Entity"
    };

    /// <summary>
    /// A dummy custom entity that can be used to for testing unstructured data
    /// deletions. The entity should not be deleted, but you can assign unstructured data
    /// relations to it.
    /// type.
    /// </summary>
    public TestCustomEntityInfo CustomEntityForUnstructuredDataTests { get; set; } = new TestCustomEntityInfo()
    {
        CustomEntityDefinitionCode = TestCustomEntityDefinition.Code,
        UrlSlug = "entity-unstructured-data",
        Title = "Test entity for testing unstructured data dependencies"
    };

    /// <summary>
    /// A test user area with a single role.
    /// </summary>
    public TestUserAreaInfo TestUserArea1 { get; set; } = new TestUserAreaInfo()
    {
        Definition = new Shared.TestUserArea1(),
        UserAreaCode = Shared.TestUserArea1.Code,
        RoleA = new TestRoleInfo()
        {
            RoleCode = Shared.TestUserArea1RoleA.Code
        },
        RoleB = new TestRoleInfo()
        {
            RoleCode = Shared.TestUserArea1RoleB.Code
        }
    };

    /// <summary>
    /// A secondary test user area with a single role.
    /// </summary>
    public TestUserAreaInfo TestUserArea2 { get; set; } = new TestUserAreaInfo()
    {
        Definition = new Shared.TestUserArea2(),
        UserAreaCode = Shared.TestUserArea2.Code,
        RoleA = new TestRoleInfo()
        {
            RoleCode = Shared.TestUserArea2RoleA.Code
        },
        RoleB = new TestRoleInfo()
        {
            RoleCode = Shared.TestUserArea2RoleB.Code
        }
    };
}
