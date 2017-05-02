using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web
{
    public class RichTextDisplayModel : IPageModuleDisplayModel
    {
        public HtmlString RawHtml { get; set; }
    }
}