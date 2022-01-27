namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Information about the full directory path and it's position in the directory 
    /// heirachy. This table is automatically updated whenever changes are made to the page 
    /// directory heirarchy and should be treated as read-only.
    /// </summary>
    public class PageDirectoryPath
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The parent <see cref="PageDirectory"/>. This can only be null for the 
        /// root directory.
        /// </summary>
        public virtual PageDirectory PageDirectory { get; set; }

        /// <summary>
        /// The zero-based depth of the directory in the heirarchy structure, where
        /// 0 is the depth of the singular root node.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The complete path of the directory in the heirarchy up to and including 
        /// this directory, without a leading or trailing slash e.g. "parent-directory/child-directory"
        /// </summary>
        public string FullUrlPath { get; set; }
    }
}