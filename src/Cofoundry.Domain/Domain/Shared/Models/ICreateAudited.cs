using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marks an entity that has audit data for entity creation.
    /// </summary>
    public interface ICreateAudited
    {
        /// <summary>
        /// Simple audit data for entity creation.
        /// </summary>
        CreateAuditData AuditData { get; set; }
    }
}
