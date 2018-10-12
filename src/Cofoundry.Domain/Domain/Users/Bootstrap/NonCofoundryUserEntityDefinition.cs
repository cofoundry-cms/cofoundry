using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Entity definition to represent non-cofoundry users which is required
    /// to be able to manage thier permissions separately in the system.
    /// </summary>
    public class NonCofoundryUserEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFUSN";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "User (Non Cofoundry)"; } }
    }
}
