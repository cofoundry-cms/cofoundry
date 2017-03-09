using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to determine if a webdirectory UrlPath is unique
    /// within its parent directory.
    /// </summary>
    public class IsWebDirectoryPathUniqueQuery : IQuery<bool>
    {
        /// <summary>
        /// Optional Id of the web directory being updated so it can
        /// be removed from the check.
        /// </summary>
        public int? WebDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent directory. The UrlPath will
        /// checked for uniqueness within this directory.
        /// </summary>
        public int? ParentWebDirectoryId { get; set; }

        /// <summary>
        /// The url path to check for uniqueness.
        /// </summary>
        public string UrlPath { get; set; }
    }
}
