using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Factory to create the default ruleset for the sanitizer. This uses the 
    /// default settings from Ganss.XSS.HtmlSanitizer, but with the addition
    /// of the class attribute. See https://github.com/mganss/HtmlSanitizer
    /// for more info.
    /// </summary>
    public class DefaultHtmlSanitizationRuleSetFactory : IDefaultHtmlSanitizationRuleSetFactory
    {
        private Lazy<HtmlSanitizationRuleSet> _defaultRulset = new Lazy<HtmlSanitizationRuleSet>(Initizalize);
        
        public IHtmlSanitizationRuleSet Create()
        {
            return _defaultRulset.Value;
        }

        private static HtmlSanitizationRuleSet Initizalize()
        {
            var ruleSet = new HtmlSanitizationRuleSet();

            ruleSet.PermittedAttributes = Ganss.XSS.HtmlSanitizer.DefaultAllowedAttributes;
            ruleSet.PermittedCssProperties = Ganss.XSS.HtmlSanitizer.DefaultAllowedCssProperties;
            ruleSet.PermittedSchemes = Ganss.XSS.HtmlSanitizer.DefaultAllowedSchemes;
            ruleSet.PermittedTags = Ganss.XSS.HtmlSanitizer.DefaultAllowedTags;
            ruleSet.PermittedUriAttributes = Ganss.XSS.HtmlSanitizer.DefaultUriAttributes;
            ruleSet.PermittedAttributes.Add("class");
            ruleSet.PermittedSchemes.Add("mailto");

            return ruleSet;
        }
    }
}
