using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Very basic information about a child and it's root entity.
    /// </summary>
    public class ChildEntityMicroSummary : RootEntityMicroSummary
    {
        public int ChildEntityId { get; set; }
    }
}
