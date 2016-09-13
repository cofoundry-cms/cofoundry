using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.Web
{

    public class HtmlSanitizationRuleSet
    {
        #region constructor

        public HtmlSanitizationRuleSet(IEnumerable<PermittedTag> permittedTags)
        {
            PermittedTags = permittedTags ?? Enumerable.Empty<PermittedTag>();
        }

        #endregion

        public Action<HtmlDocument> OnHtmlSanitized { get; set; }

        public IEnumerable<PermittedTag> PermittedTags { get; set; }
    }
}
