using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This enumeration maps (by integer id) to the work flow statuses table in the database,
    /// apart from 'none'.
    /// </summary>
    public enum WorkFlowStatus
    {
        None = 0,
        Draft = 1,
        Published = 4,
    }
}
