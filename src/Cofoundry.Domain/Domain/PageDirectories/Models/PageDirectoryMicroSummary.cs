namespace Cofoundry.Domain
{
    /// <summary>
    /// A minimal projection of a page directory that includes only 
    /// basic information.
    /// </summary>
    public class PageDirectoryMicroSummary
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the parent directory. This can only be null for the 
        /// root directory.
        /// </summary>
        public int? ParentPageDirectoryId { get; set; }

        /// <summary>
        /// The zero base depth of this directory in the tree stucture
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The full (relative) url of this directory with the leading
        /// slash, but excluding the trailing slash e.g. "/my-directory".
        /// </summary>
        public string FullUrlPath { get; set; }
    }
}
