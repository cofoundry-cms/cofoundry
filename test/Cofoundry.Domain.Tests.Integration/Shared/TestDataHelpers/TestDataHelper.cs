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
        private readonly DbDependentFixture _dbDependentFixture;

        public TestDataHelper(DbDependentFixture dbDependentFixture)
        {
            _dbDependentFixture = dbDependentFixture;

            PageDirectories = new PageDirectoryTestDataHelper(_dbDependentFixture);
            Pages = new PageTestDataHelper(_dbDependentFixture);
        }

        /// <summary>
        /// Used to make it easier to create or reference page 
        /// directories in test fixtures.
        /// </summary>
        public PageDirectoryTestDataHelper PageDirectories { get; private set; }

        /// <summary>
        /// Used to make it easier to create pages in test fixtures.
        /// </summary>
        public PageTestDataHelper Pages { get; private set; }
    }
}
