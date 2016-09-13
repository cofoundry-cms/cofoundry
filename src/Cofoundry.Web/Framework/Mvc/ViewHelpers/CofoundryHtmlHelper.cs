using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class CofoundryHtmlHelper : ICofoundryHtmlHelper
    {
        /// <summary>
        /// Returns a class attribute with the specified class if the condition is met; otherwise an empty string.
        /// </summary>
        /// <param name="condition">Condition to check to see whtehr the class should be output or not.</param>
        /// <param name="cls">The class to apply if the condition is met.</param>
        /// <returns>HtmlString in the format 'class="{cls}"'.</returns>
        public HtmlString ClassIf(bool condition, string cls, string elseCls = null)
        {
            string formatStr ="class=\"{0}\"";
            string s = null;
            if (condition)
            {
                s = string.Format(formatStr, cls);
            }
            else if (elseCls != null)
            {
                s = string.Format(formatStr, elseCls);
            }

            return new HtmlString(s ?? string.Empty);
        }

        /// <summary>
        /// Returns the specified string value if the condition is met.
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="passContent">String to insert if the condition is true</param>
        /// <param name="failContent">Optional string to insert if the condition is false</param>
        /// <returns>The value if the condition is true; otherwise an empty IHtmlString.</returns>
        public IHtmlString TextIf(bool condition, string passContent, string failContent = null)
        {
            if (condition)
            {
                return new HtmlString(passContent);
            }
            else if (failContent != null)
            {
                return new HtmlString(failContent);
            }

            return new HtmlString(string.Empty);
        }
    }
}