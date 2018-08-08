using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Miscellaneous helper functions to make working work html views easier.
    /// </summary>
    public interface ICofoundryHtmlHelper
    {
        /// <summary>
        /// Returns a class attribute with the specified class if the condition is met; otherwise an empty string.
        /// </summary>
        /// <param name="condition">Condition to check to see whtehr the class should be output or not.</param>
        /// <param name="cls">The class to apply if the condition is met.</param>
        /// <param name="elseCls">Optional class to apply if the condition is not met.</param>
        /// <returns>HtmlString in the format 'class="{cls}"'.</returns>
        IHtmlContent ClassIf(bool condition, string cls, string elseCls = null);

        /// <summary>
        /// Returns the specified string value if the condition is met.
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="passContent">String to insert if the condition is true</param>
        /// <param name="failContent">Optional string to insert if the condition is false</param>
        /// <returns>The value if the condition is true; otherwise an empty IHtmlString.</returns>
        IHtmlContent TextIf(bool condition, string passContent, string failContent = null);
    }
}