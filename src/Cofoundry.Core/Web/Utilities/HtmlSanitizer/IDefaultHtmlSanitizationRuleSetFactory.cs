using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Web;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Factory to create the default ruleset for the sanitizer
    /// </summary>
    public interface IDefaultHtmlSanitizationRuleSetFactory
    {
        HtmlSanitizationRuleSet Create();
    }
}
