using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFDOC";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Document"; } }
    }
}
