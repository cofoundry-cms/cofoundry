using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cofoundry.Web
{
    public static class TemplateRegionTagBuilderHelper
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
            string pageBlocksHtml, 
            string wrappingTagName,
            bool allowMultiplePageBlocks,
            Dictionary<string, string> additonalHtmlAttributes,
            Dictionary<string, string> editorAttributes = null
            )
        {
            const string DEFAULT_TAG ="div";

            var parser = new HtmlParser();
            var document = parser.ParseDocument(pageBlocksHtml.Trim());

            IElement wrapper;
            // No need to wrap if its a single page block with a single outer node.
            if (!allowMultiplePageBlocks && document.Body.Children.Length == 1 && wrappingTagName == null)
            {
                wrapper = document.Body.Children.First();
            }
            else
            {
                wrapper = document.CreateElement(wrappingTagName ?? DEFAULT_TAG);
                AngleSharpHelper.WrapChildren(document.Body, wrapper);
            }

            AngleSharpHelper.MergeAttributes(wrapper, additonalHtmlAttributes);

            if (editorAttributes != null)
            {
                AngleSharpHelper.MergeAttributes(wrapper, editorAttributes);
            }

            return wrapper.OuterHtml;
        }
    }
}