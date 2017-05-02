using Cofoundry.Core.Web;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public static class TemplateSectionTagBuilderHelper
    {
        /// <summary>
        /// Parses an anonymous object as key-value pairs of html attributes.
        /// </summary>
        public static Dictionary<string, string> ParseHtmlAttributesFromAnonymousObject(object anonymousObject)
        {
            if (anonymousObject != null)
            {
                var attributes = new Dictionary<string, string>();

                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
                {
                    var value = propertyDescriptor.GetValue(anonymousObject);
                    attributes.Add(propertyDescriptor.Name, Convert.ToString(value));
                }

                return attributes;
            }

            return null;
        }

        public static string WrapInTag(
            string modulesHtml, 
            string wrappingTagName,
            bool allowMultipleModules,
            Dictionary<string, string> additonalHtmlAttributes,
            Dictionary<string, string> editorAttributes = null
            )
        {
            const string DEFAULT_TAG ="div";

            var html = new HtmlDocument();
            html.LoadHtml(modulesHtml.Trim());

            HtmlNode wrapper;

            // No need to wrap if its a single module with a single outer node.
            if (!allowMultipleModules && html.DocumentNode.ChildNodes.Count == 1 && wrappingTagName == null)
            {
                wrapper = html.DocumentNode.ChildNodes.First();
            }
            else
            {
                var wrap = new HtmlDocument();
                wrapper = wrap.CreateElement(wrappingTagName ?? DEFAULT_TAG);
                wrapper.InnerHtml = modulesHtml;
            }

            wrapper.MergeAttributes(additonalHtmlAttributes);

            if (editorAttributes != null)
            {
                wrapper.MergeAttributes(editorAttributes);
            }

            return wrapper.OuterHtml;
        }
    }
}