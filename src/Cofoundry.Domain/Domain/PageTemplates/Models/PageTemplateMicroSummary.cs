using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateMicroSummary
    {
        public int PageTemplateId { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public Type CustomEntityModelType { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public bool IsArchived { get; set; }
    }
}
