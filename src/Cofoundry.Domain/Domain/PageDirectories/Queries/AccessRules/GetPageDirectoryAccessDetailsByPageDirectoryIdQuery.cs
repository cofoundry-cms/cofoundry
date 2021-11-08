using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all access rules associated with a directory, including those inherited from
    /// parent directories.
    /// </summary>
    public class GetPageDirectoryAccessDetailsByPageDirectoryIdQuery : IQuery<PageDirectoryAccessRuleSetDetails>
    {
        public GetPageDirectoryAccessDetailsByPageDirectoryIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified <paramref name="pageDirectoryId"/>.
        /// </summary>
        /// <param name="pageDirectoryId">
        /// Database id of the page directory to get access rules for.
        /// </param>
        public GetPageDirectoryAccessDetailsByPageDirectoryIdQuery(int pageDirectoryId)
        {
            PageDirectoryId = pageDirectoryId;
        }

        /// <summary>
        /// Database id of the page directory to get access rules for.
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
