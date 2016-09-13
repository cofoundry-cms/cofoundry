using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateFileInfo
    {
        public PageTemplateFileSection[] Sections { get; set; }

        public string CustomEntityModelType { get; set; }

        public PageType PageType { get; set; }

        public CustomEntityDefinitionMicroSummary CustomEntityDefinition { get; set; }
    }
}
