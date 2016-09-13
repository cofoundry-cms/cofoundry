using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageTemplateEntityDefinition : IEntityDefinition
    {
        public static string DefinitionCode = "COFPTL";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Page Template"; } }
    }
}
