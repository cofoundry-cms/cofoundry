using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// An HtmlSanitizer that uses the Ganss.XSS.HtmlSanitizer
    /// library for cleaning input. Unfortunately the ganss HtmlSanitizer 
    /// shares the same class name, hopefully this doesn't cause too much 
    /// confusion.
    /// </summary>
    public class HtmlSanitizer : IHtmlSanitizer
    {
        private Ganss.XSS.HtmlSanitizer _defaultSanitizer;
        private readonly string _defaultBaseUrl = null;

        public HtmlSanitizer(
            IDefaultHtmlSanitizationRuleSetFactory defaultHtmlSanitizationRuleSetFactory
            )
        {
            var defaultHtmlSanitizationRuleSet = defaultHtmlSanitizationRuleSetFactory.Create();
            _defaultSanitizer = CreateSanitizer(defaultHtmlSanitizationRuleSet);
            _defaultBaseUrl = GetBaseUrl(defaultHtmlSanitizationRuleSet);
        }

        public virtual string Sanitize(IHtmlContent source)
        {
            if (source == null) return string.Empty;
            IHtmlSanitizationRuleSet ruleSet = null;
            if (source is ICustomSanitizationHtmlString)
            {
                ruleSet = ((ICustomSanitizationHtmlString)source).SanitizationRuleSet;
            }

            return Sanitize(source.ToString()?.Trim(), ruleSet);
        }

        public virtual string Sanitize(string source, IHtmlSanitizationRuleSet ruleSet = null)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            string result;

            if (ruleSet == null)
            {
                result = _defaultSanitizer.Sanitize(source, _defaultBaseUrl);
            }
            else
            {
                var sanitizer = CreateSanitizer(ruleSet);
                var baseUrl = GetBaseUrl(ruleSet);

                result = sanitizer.Sanitize(source, baseUrl);
            }

            return result;
        }

        /// <summary>
        /// Remove HTML tags from string
        /// </summary>
        public virtual string StripHtml(HtmlString content)
        {
            if (content == null) return null;

            return StripHtml(content?.Value);
        }

        /// <summary>
        /// Remove HTML tags from string
        /// </summary>
        /// <remakrs>
        /// See http://www.dotnetperls.com/remove-html-tags
        /// </remakrs>
        public virtual string StripHtml(string source)
        {
            if (source == null) return string.Empty;

            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        protected string GetBaseUrl(IHtmlSanitizationRuleSet ruleSet)
        {
            var ganssRuleSet = ruleSet as IGanssHtmlSanitizationRuleSet;
            if (ganssRuleSet == null) return null;

            return ganssRuleSet.BaseUrl;
        }

        protected Ganss.XSS.HtmlSanitizer CreateSanitizer(IHtmlSanitizationRuleSet ruleSet)
        {
            var sanitizer = new Ganss.XSS.HtmlSanitizer(
                ruleSet.PermittedTags,
                ruleSet.PermittedSchemes,
                ruleSet.PermittedAttributes,
                ruleSet.PermittedUriAttributes,
                ruleSet.PermittedCssProperties
                );

            var gnassRuleSet = ruleSet as IGanssHtmlSanitizationRuleSet;

            if (gnassRuleSet != null)
            {
                gnassRuleSet.Initialize(sanitizer);
            }

            return sanitizer;
        }
    }
}
