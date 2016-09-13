using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public interface IEntityVersion
    {
        int WorkFlowStatusId { get; set; }
    }
}
