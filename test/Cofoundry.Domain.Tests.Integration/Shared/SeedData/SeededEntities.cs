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
        /// An 80x80 jpg.
        /// </summary>
        public int TestImageId { get; set; }

        /// <summary>
        /// An existing tag which can be used in combination with a unique tag to test new and existing tag references.
        /// </summary>
        public TestTagInfo TestTag { get; } = new TestTagInfo();

        public class TestPageTemplateInfo
        {
            public int PageTemplateId { get; set; }

            /// <summary>
            /// The id of the "Body" page region in the template.
            /// </summary>
            public int BodyPageTemplateRegionId { get; set; }
        }

        public class TestCustomEntityPageTemplateInfo : TestPageTemplateInfo
        {
            /// <summary>
            /// The id of the "Custom Entity Body" page region in the template.
            /// </summary>
            public int CustomEntityBodyPageTemplateRegionId { get; set; }
        }

        public class TestTagInfo
        {
            public int TagId { get; set; }

            public string TagText { get; } = "Test";
        }
    }
}
