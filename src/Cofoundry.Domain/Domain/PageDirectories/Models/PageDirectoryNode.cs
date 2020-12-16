using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a page directory in a hierarchy of tree nodes, containing parent 
    /// and child tree node 
    /// navigations properties.
    /// </summary>
    public class PageDirectoryNode : ICreateAudited
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
        /// The parent directory in the site tree structure.
        /// </summary>
        /// <remarks>
        /// This property is ignored in serialization to prevent duplication 
        /// of data.
        /// </remarks>
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public PageDirectoryNode ParentPageDirectory { get; set; }

        /// <summary>
        /// Child pages in the site tree hierarchy.
        /// </summary>
        public ICollection<PageDirectoryNode> ChildPageDirectories { get; set; }

        /// <summary>
        /// The path of this directory (excluding parent directory path)
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The number of pages directly associated with this 
        /// directory i.e. excludes child directory pages.
        /// </summary>
        public int NumPages { get; set; }
        
        /// <summary>
        /// The zero base depth of this directory in the tree stucture
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The full (relative) url of this directory
        /// excluding the trailing slash.
        /// </summary>
        public string FullUrlPath { get; set; }

        public CreateAuditData AuditData { get; set; }

        /// <summary>
        /// Flattens out the Node and it's children into a single enumerable.
        /// </summary>
        /// <returns>This instance and any child nodes recursively.</returns>
        public IEnumerable<PageDirectoryNode> Flatten()
        {
            yield return this;

            foreach (var childNode in ChildPageDirectories)
            foreach (var flattenedNode in childNode.Flatten())
            {
                yield return flattenedNode;
            }
        }
    }
}
