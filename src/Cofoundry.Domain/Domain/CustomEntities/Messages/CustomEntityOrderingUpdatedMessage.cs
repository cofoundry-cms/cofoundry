using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityOrderingUpdatedMessage : ICustomEntityContentUpdatedMessage
    {
        /// <summary>
        /// Ids of the custom entity that have had thier ordering changed
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Definition code of the custom entity that the content change affects
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        public bool HasPublishedVersionChanged
        {
            get { return true; }
        }
    }
}
