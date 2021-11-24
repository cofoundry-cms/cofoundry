using Microsoft.AspNetCore.Http;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page directory is deleted. When a directory
    /// is deleted, it's child directories are also deleted which will
    /// generate multiple messages in the same batch, one for each directory 
    /// deleted.
    /// </summary>
    public class PageDirectoryDeletedMessage
    {
        /// <summary>
        /// Id of the page directory that has been deleted.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The full (relative) url of the deleted directory with the leading
        /// slash, but excluding the trailing slash e.g. "/parent-directory/child-directory".
        /// </summary
        public string FullUrlPath { get; set; }
    }
}
