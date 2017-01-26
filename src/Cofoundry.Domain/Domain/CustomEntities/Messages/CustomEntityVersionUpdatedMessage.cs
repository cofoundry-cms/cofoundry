using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a custom entity version has been update
    /// </summary>
    public class CustomEntityVersionUpdatedMessage
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
        /// Id of the custom entity version that has been updated
        /// </summary>
        public int CustomEntityVersionId { get; set; }
    }
}
