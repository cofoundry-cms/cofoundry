using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IVersionRoute
    {
        int VersionId { get; set; }

        WorkFlowStatus WorkFlowStatus { get; set; }

        string Title { get; set; }

        DateTime CreateDate { get; set; }
    }
}
