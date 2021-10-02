using System;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create or reference domain 
    /// entities in test fixtures.
    /// </summary>
    public class TestDataHelper
    {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly SeededEntities _seededEntities;

        public TestDataHelper(
            IServiceProvider rootServiceProvider,
            SeededEntities seededEntities
            )
        {
            _rootServiceProvider = rootServiceProvider;
            _seededEntities = seededEntities;
        }

        /// <summary>
        /// Used to make it easier to create or reference page 
        /// directories in test fixtures.
        /// </summary>
        public PageDirectoryTestDataHelper PageDirectories()
        {
            return new PageDirectoryTestDataHelper(_rootServiceProvider);
        }

        /// <summary>
        /// Used to make it easier to create or reference page 
        /// templates in test fixtures.
        /// </summary>
        public PageTemplateTestDataHelper PageTemplates()
        {
            return new PageTemplateTestDataHelper(_rootServiceProvider);
        }

        /// <summary>
        /// Used to make it easier to create pages in test fixtures.
        /// </summary>
        public PageTestDataHelper Pages()
        {
            return new PageTestDataHelper(_rootServiceProvider, _seededEntities);
        }

        /// <summary>
        /// Used to make it easier to create custom entities in test fixtures.
        /// </summary>
        public CustomEntityTestDataHelper CustomEntities()
        {
            return new CustomEntityTestDataHelper(_rootServiceProvider);
        }
    }
}
