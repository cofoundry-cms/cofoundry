using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UpdateAuditData : CreateAuditData
    {
        public UserMicroSummary Updater { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
