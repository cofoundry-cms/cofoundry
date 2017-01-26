using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a custom entity draft version is deleted
    /// </summary>
    public class CustomEntityDraftVersionDeletedMessage : ICustomEntityContentUpdatedMessage
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
        /// Id of the version that was deleted
        /// </summary>
        public int CustomEntityVersionId { get; set; }

        /// <summary>
        /// Only drafts can be deleted so this is always false
        /// </summary>
        public bool HasPublishedVersionChanged
        {
            get { return false; }
        }
    }
}
