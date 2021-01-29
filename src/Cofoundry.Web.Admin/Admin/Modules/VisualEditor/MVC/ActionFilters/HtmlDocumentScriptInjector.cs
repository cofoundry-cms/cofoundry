using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// Use to injects script, CSS or meta content into an HTML 
    /// document.
    /// </summary>
    public class HtmlDocumentScriptInjector : IHtmlDocumentScriptInjector
    {
        const string HEAD_TAG_START = "<head>";
        const string HEAD_TAG_END = "</head>";
        const string BODY_TAG_START = "<body";
        const string BODY_TAG_END = "</body>";

        /// <summary>
        /// Injects scripts, CSS or meta content into an HTML 
        /// document, either in the head or just before the closing
        /// body tag. The HTML document must at least contain a body
        /// tag, but otherwise no validation if performed.
        /// </summary>
        /// <param name="html">
        /// The HTML document to inject. This is not parsed or validated, 
        /// but must at least contain a body tag. If no head tag is present
        /// and headScript is supplied, then a set of head tags will automatically 
        /// be addded.
        /// </param>
        /// <param name="headScript">
        /// String content to add into the head of the document, just before the
        /// closing head tag. If no head tags are present, a set will be added
        /// automatically. Can be null if no head content is to be added.
        /// </param>
        /// <param name="bodyScript">
        /// A script to add at the end of the document body, just before the closing 
        /// body tag. Can be null if no body content is to be added.
        /// </param>
        /// <returns>The modified html document.</returns>
        public string InjectScripts(string html, string headScript, string bodyScript)
        {
            html = InjectHeadScript(html, headScript);

            if (string.IsNullOrWhiteSpace(bodyScript))
            {
                // Nothing more to do
                return html;
            }

            var insertBodyIndex = html.LastIndexOf(BODY_TAG_END, StringComparison.OrdinalIgnoreCase);

            // Early return if no body tag found
            if (insertBodyIndex < 0) return html;

            html = html.Substring(0, insertBodyIndex)
                + bodyScript
                + Environment.NewLine
                + html.Substring(insertBodyIndex);

            return html;
        }

        private string InjectHeadScript(string html, string headScript)
        {
            if (string.IsNullOrWhiteSpace(headScript)) return html;

            var insertHeadIndex = html.IndexOf(HEAD_TAG_END, StringComparison.OrdinalIgnoreCase);

            if (insertHeadIndex > 0)
            {
                html = html.Substring(0, insertHeadIndex)
                    + headScript
                    + Environment.NewLine
                    + html.Substring(insertHeadIndex);
            }
            else
            {
                // No head, let's add one.
                var bodyStartIndex = html.IndexOf(BODY_TAG_START, StringComparison.OrdinalIgnoreCase);

                if (bodyStartIndex > 0)
                {
                    html = html.Substring(0, bodyStartIndex)
                        + HEAD_TAG_START
                        + Environment.NewLine
                        + headScript
                        + Environment.NewLine
                        + HEAD_TAG_END
                        + Environment.NewLine
                        + html.Substring(bodyStartIndex);
                }
            }

            return html;
        }
    }
}
