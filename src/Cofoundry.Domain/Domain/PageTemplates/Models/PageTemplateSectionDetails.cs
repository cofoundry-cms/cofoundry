using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateSectionDetails : ICreateAudited
    {
        public int PageTemplateSectionId { get; set; }

        public int PageTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsCustomEntitySection { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
