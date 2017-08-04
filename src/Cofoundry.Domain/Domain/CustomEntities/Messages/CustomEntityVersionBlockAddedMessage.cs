using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page block has been added to a custom entity
    /// </summary>
    public class CustomEntityVersionBlockAddedMessage : ICustomEntityContentUpdatedMessage
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
        /// Id of the newly added page block
        /// </summary>
        public int CustomEntityVersionPageBlockId { get; set; }

        /// <summary>
        /// Always false because only a draft version can be edited
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }
    }
}
