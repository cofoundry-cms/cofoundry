using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web
{
    public class RichTextDisplayModel : IPageBlockTypeDisplayModel
    {
        public HtmlString RawHtml { get; set; }
    }
}