using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateSummary : ICreateAudited
    {
        public int PageTemplateId { get; set; }

        public string FileName { get; set; }

        public string Name { get; set; }

        public PageType PageType { get; set; }

        public int NumSections { get; set; }

        public int NumPages { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
