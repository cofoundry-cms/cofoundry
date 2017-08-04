using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that the content of a custom entity has changed, e.g. any content elements like
    /// page blocks or data model properties that for example might require a change to a search index.
    /// </summary>
    public interface ICustomEntityContentUpdatedMessage
    {
        /// <summary>
        /// Id of the custom entity that the content change affects
        /// </summary>
        int CustomEntityId { get; }

        /// <summary>
        /// Definition code of the custom entity that the content change affects
        /// </summary>
        string CustomEntityDefinitionCode { get; }

        /// <summary>
        /// Indicates whether the content change affected the published version.
        /// </summary>
        bool HasPublishedVersionChanged { get; }
    }
}
