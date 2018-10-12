using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SettingsEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFSET";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Settings"; } }
    }
}
