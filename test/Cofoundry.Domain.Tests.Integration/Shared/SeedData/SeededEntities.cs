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
        public int TestPageTemplateId { get; set; }

        /// <summary>
        /// A test custom entity template for custom entity 
        /// "TestCustomEntityDefinition" with a page region named "Page Body" and
        /// a custom entity region named "Custom Entity Body". Both regions
        /// accepts multiple blocks of any type.
        /// </summary>
        public int TestCustomEntityPageTemplateId { get; internal set; }

        /// <summary>
        /// An 80x80 jpg.
        /// </summary>
        public int TestImageId { get; set; }

        /// <summary>
        /// An existing tag which can be used in combination with a unique tag to test new and existing tag references.
        /// </summary>
        public string TestTag { get; } = "Test";

        public int TestTagId { get; set; }
    }
}
