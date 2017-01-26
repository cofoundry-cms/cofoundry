using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a new custom entity has been added
    /// </summary>
    public class CustomEntityAddedMessage : ICustomEntityContentUpdatedMessage
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
        /// True if the new custom entity was published
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }
    }
}
