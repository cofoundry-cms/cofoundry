using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that the content of a page has changed, e.g. any content elements like
    /// modules or other page properties that for example might require a change to a search index.
    /// </summary>
    public interface IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page entity that the content change affects
        /// </summary>
        int PageId { get; }

        /// <summary>
        /// Indicates whether the content change affected the published version.
        /// </summary>
        bool HasPublishedVersionChanged { get; }
    }
}
