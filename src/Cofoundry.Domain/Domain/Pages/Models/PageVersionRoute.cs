using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageVersionRoute : IVersionRoute
    {
        public int VersionId { get; set; }

        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public int PageTemplateId { get; set; }

        public bool HasPageModuleSections { get; set; }

        public bool HasCustomEntityModuleSections { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }
    }
}
