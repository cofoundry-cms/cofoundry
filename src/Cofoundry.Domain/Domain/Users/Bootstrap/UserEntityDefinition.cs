using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The main entity definition for users. In terms of permissions this
    /// represents Cofoundry Admin users.
    /// </summary>
    public class UserEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFUSR";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "User (Cofoundry)"; } }
    }
}
