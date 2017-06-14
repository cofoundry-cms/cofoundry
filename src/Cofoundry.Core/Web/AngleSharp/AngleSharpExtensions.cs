using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
   public static class AngleSharpHelper
   {
        /// <summary>
        /// Wraps all child elements with an outer container element.
        /// </summary>
        /// <param name="parentElement">The element containing the child elements to wrap.</param>
        /// <param name="wrapperElement">The element to wrap the child elements in.</param>
        public static void WrapChildren(IElement parentElement, IElement wrapperElement)
        {
            if (wrapperElement == null) throw new ArgumentNullException(nameof(wrapperElement));

            // The parent element could be null if the result of a query
            if (parentElement == null) return;

            while (parentElement.Children?.Length > 0)
            {
                var child = parentElement.Children[0];
                child.Remove();

                wrapperElement.AppendChild(child);
            }

            parentElement.AppendChild(wrapperElement);
        }


        /// <summary>
        /// Merges existing attributes on a node with the specified attribute 
        /// collection, adding attributes that don't exist or replacing existing ones.
        /// </summary>
        /// <param name="element">Element to act on.</param>
        /// <param name="dataAttributes">Attributes to merge.</param>
        public static void MergeAttributes(IElement element, IDictionary<string, string> dataAttributes)
        {
            if (dataAttributes == null) return;

            foreach (var attr in dataAttributes)
            {
                if (attr.Key.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    MergeCss(element, attr.Value);
                }
                else
                {
                    element.SetAttribute(attr.Key, attr.Value);
                }
            }
        }

        /// <summary>
        /// Merges the specified css classes with those already on the element.
        /// </summary>
        /// <param name="element">Element to act on.</param>
        /// <param name="css">Space delimited list of css classes to merge.</param>
        public static void MergeCss(IElement element, string css)
        {
            var cssDelimiter = new char[] { ' ' };
            var newClasses = StringHelper.SplitAndTrim(css, cssDelimiter);

            element.ClassList.Add(newClasses.ToArray());
        }
    }
}
