using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create or reference domain 
    /// entities in test fixtures.
    /// </summary>
    public class TestDataHelper
    {
        public TestDataHelper(DbDependentFixture dbDependentFixture)
        {
            DbDependentFixture = dbDependentFixture;
        }

        public DbDependentFixture DbDependentFixture { get; private set; }

        /// <summary>
        /// Used to make it easier to create or reference page 
        /// directories in test fixtures.
        /// </summary>
        public PageDirectoryTestDataHelper PageDirectories()
        {
            return new PageDirectoryTestDataHelper(DbDependentFixture);
        }

        /// <summary>
        /// Used to make it easier to create pages in test fixtures.
        /// </summary>
        public PageTestDataHelper Pages()
        {
            return new PageTestDataHelper(DbDependentFixture);
        }
    }
}
