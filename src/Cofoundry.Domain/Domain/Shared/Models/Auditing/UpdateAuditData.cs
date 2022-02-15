using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple audit data for updatable entities.
    /// </summary>
    public class UpdateAuditData : CreateAuditData
    {
        /// <summary>
        /// The user that last updated the entity. When first created
        /// the updater will be assigned to the user that created the 
        /// entity, ensuring this property always has a value.
        /// </summary>
        public UserMicroSummary Updater { get; set; }

        /// <summary>
        /// The date the entity was last updated. When first created
        /// this value will be set tot he creation date, ensuring this
        /// property always has a value.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
