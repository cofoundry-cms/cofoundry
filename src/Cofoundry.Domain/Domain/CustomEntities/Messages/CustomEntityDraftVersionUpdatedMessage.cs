using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a custom entity draft version has been updated
    /// </summary>
    public class CustomEntityDraftVersionUpdatedMessage : ICustomEntityContentUpdatedMessage
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
        /// Always false because updating a draft cannot change the published version
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }

        /// <summary>
        /// Id of the version that was updated
        /// </summary>
        public int CustomEntityVersionId { get; set; }
    }
}
