using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateDetails : ICreateAudited
    {
        public int PageTemplateId { get; set; }

        public string FileName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FullPath { get; set; }

        public int NumPages { get; set; }

        public CustomEntityDefinitionMicroSummary CustomEntityDefinition { get; set; }

        public string CustomEntityModelType { get; set; }

        public PageType PageType { get; set; }

        public PageTemplateSectionDetails[] Sections { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
