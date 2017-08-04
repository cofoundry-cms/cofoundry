using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page block has been updated on a custom entity
    /// </summary>
    public class CustomEntityVersionBlockUpdatedMessage : ICustomEntityContentUpdatedMessage
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
        /// Id of the version that contains the block that was updated
        /// </summary>
        public int CustomEntityVersionBlockId { get; set; }

        /// <summary>
        /// Always false because only a draft version can be edited
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }
    }
}
