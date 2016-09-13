using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Domain
{
    public class PageSearchResult
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public IHtmlString FoundText { get; set; }
    }
}
