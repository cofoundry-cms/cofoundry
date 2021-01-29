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
    public interface IHtmlDocumentScriptInjector
    {
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
        string InjectScripts(string html, string headScript, string bodyScript);
    }
}
