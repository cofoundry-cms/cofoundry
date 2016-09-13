using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.Web
{
    public class PermittedTag
    {
        public PermittedTag(string tag, params string[] attributes)
        {
            Tag = tag;
            PermittedAttributes = attributes ?? new string[0];
        }

        public string Tag { get; set; }
        public string[] PermittedAttributes { get; set; }

        public Action<HtmlNode> TagAction { get; set; }
    }
}
