using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marks a model that contains simple audit data for
    /// when the entity was last updated.
    /// </summary>
    public interface IUpdateAudited
    {
        /// <summary>
        /// Simple audit data for the last entity update.
        /// </summary>
        UpdateAuditData AuditData { get; set; }
    }
}
