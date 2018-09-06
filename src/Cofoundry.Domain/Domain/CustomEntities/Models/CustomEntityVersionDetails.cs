using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionDetails
    {
        public int CustomEntityVersionId { get; set; }

        /// <summary>
        /// A display-friendly version number that indicates
        /// it's position in the hisotry of all verions of a specific
        /// custom entity. E.g. the first version for a custom entity 
        /// is version 1 and  the 2nd is version 2. The display version 
        /// is unique per custom entity.
        /// </summary>
        public int DisplayVersion { get; set; }

        public string Title { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }

        public ICustomEntityDataModel Model { get; set; }

        public ICollection<CustomEntityPage> Pages { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
