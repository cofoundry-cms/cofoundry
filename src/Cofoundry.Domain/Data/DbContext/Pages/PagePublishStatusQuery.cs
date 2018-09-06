using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Lookup cache used for quickly finding the correct version for a
    /// specific publish status query e.g. 'Latest', 'Published', 
    /// 'PreferPublished'. These records are generated when pages
    /// are published or unpublished.
    /// </summary>
    public partial class PagePublishStatusQuery
    {
        /// <summary>
        /// Id of the page this record represents. Forms a key
        /// with the PublishStatusQueryId.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Numeric representation of the domain PublishStatusQuery enum.
        /// </summary>
        public short PublishStatusQueryId { get; set; }

        /// <summary>
        /// The id of the version of the page that should be displayed
        /// for the corresponding PublishStatusQueryId.
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// Page that this record represents.
        /// </summary>
        public virtual Page Page { get; set; }

        /// <summary>
        /// The version of the page that should be displayed
        /// for the corresponding PublishStatusQueryId.
        /// </summary>
        public virtual PageVersion PageVersion { get; set; }

    }
}
