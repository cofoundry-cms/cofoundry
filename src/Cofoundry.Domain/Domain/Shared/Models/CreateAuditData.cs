using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CreateAuditData
    {
        public UserMicroSummary Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
