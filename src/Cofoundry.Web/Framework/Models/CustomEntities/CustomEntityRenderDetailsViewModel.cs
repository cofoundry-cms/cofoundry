using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class CustomEntityRenderDetailsViewModel<TDisplayModel>
    {
        public int CustomEntityId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public ActiveLocale Locale { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public TDisplayModel Model { get; set; }

        public IEnumerable<CustomEntityPageSectionRenderDetails> Sections { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }
    }
}
