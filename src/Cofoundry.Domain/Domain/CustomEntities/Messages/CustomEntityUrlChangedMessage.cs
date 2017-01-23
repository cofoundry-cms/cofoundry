using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when the information that makes up a url on a custom
    /// entity has been changed (slug/locale)
    /// </summary>
    public class CustomEntityUrlChangedMessage : ICustomEntityContentUpdatedMessage
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
        /// True if the custom entity has a published version
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }
    }
}
