using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page block has been removed from a custom entity
    /// </summary>
    public class CustomEntityVersionBlockMovedMessage : ICustomEntityContentUpdatedMessage
    {
        /// <summary>
        /// Id of the custom entity that the content change affects
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Definition code of the custom entity that the content change affects
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Id of the version that contains the page block that was moved
        /// </summary>
        public int CustomEntityVersionBlockId { get; set; }

        /// <summary>
        /// Always false because only a draft version can be edited
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }
    }
}
