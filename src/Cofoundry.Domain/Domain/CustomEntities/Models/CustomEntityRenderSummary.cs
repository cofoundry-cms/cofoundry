using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A version of the renderable entity without module data. Useful
    /// for internal mapping.
    /// </summary>
    public class CustomEntityRenderSummary
    {
        public int CustomEntityId { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public int CustomEntityVersionId { get; set; }

        public int? Ordering { get; set; }

        public ActiveLocale Locale { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }

        public ICustomEntityVersionDataModel Model { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
