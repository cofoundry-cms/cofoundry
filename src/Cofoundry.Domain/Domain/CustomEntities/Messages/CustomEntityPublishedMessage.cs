using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a custom entity has gone from a draft to 
    /// publish state. This is not invoked when a new custom entity is
    /// created, to capture that event use CustomEntityAddedMessage and check
    /// the HasPublishedVersionChanged property.
    /// </summary>
    public class CustomEntityPublishedMessage : ICustomEntityContentUpdatedMessage
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
        /// True, obvs.
        /// </summary>
        public bool HasPublishedVersionChanged { get { return true; } }
    }
}
