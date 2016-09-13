using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class RawHtmlDisplayModel : IPageModuleDisplayModel
    {
        public HtmlString RawHtml { get; set; }
    }
}