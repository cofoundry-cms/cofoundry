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
    /// to be able to manage the permissions for the current user differently to
    /// regular users.
    /// </summary>
    public class CurrentUserEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFCUR";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "User (Current)"; } }
    }
}
