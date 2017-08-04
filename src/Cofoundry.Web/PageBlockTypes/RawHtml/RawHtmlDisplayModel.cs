using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web
{
    public class RawHtmlDisplayModel : IPageBlockTypeDisplayModel
    {
        public HtmlString RawHtml { get; set; }
    }
}