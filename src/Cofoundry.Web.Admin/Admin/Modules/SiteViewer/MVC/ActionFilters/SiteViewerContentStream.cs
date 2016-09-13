using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Takes an html text stream and manipulated it to add in a couple of features required to
    /// make the site viewer work.
    /// </summary>
    /// <remarks>
    /// Designed only for us with SiteViewerContentFilterAttribute. 
    /// </remarks>
    internal class SiteViewerContentStream : MemoryStream
    {
        #region constructor

        const char TAB = '\t';

        private readonly StringBuilder outputString = new StringBuilder();
        private readonly Stream outputStream = null;

        public SiteViewerContentStream(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        #endregion

        #region implementation

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Write out the entire stream to memory before manipulating it.
            outputString.Append(Encoding.UTF8.GetString(buffer));
        }

        public override void Close()
        {
            string html = outputString.ToString();

            // Check for not XML
            if (!html.StartsWith("<?xml"))
            {
                html = AddBaseTag(html);
                html = AddCofoundryJavascript(html);
            }

            // Write the string back to the response
            byte[] rawResult = Encoding.UTF8.GetBytes(html);
            outputStream.Write(rawResult, 0, rawResult.Length);

            base.Close();
            outputStream.Close();
        }

        #endregion

        #region html modifiers

        private string AddCofoundryJavascript(string html)
        {
            const string HEAD_TAG_END = "</head>";

            var insertIndex = html.IndexOf(HEAD_TAG_END, StringComparison.OrdinalIgnoreCase) - 1;

            if (insertIndex > 0)
            {
                html = html.Substring(0, insertIndex)
                    + Environment.NewLine + TAB
                    + Scripts.Render(SiteViewerRouteLibrary.Js.EventAggregator).ToString() + TAB
                    + Styles.Render(SiteViewerRouteLibrary.Css.InnerSiteViewer).ToString()
                    + html.Substring(insertIndex);
            }

            return html;
        }

        /// <summary>
        /// Adds a base tag to the html to make sure links in the content page open in the 
        /// site viewer frame.
        /// </summary>
        private string AddBaseTag(string html)
        {
            const string TARGET_TAG = "<base target=\"_top\" />";
            const string TITLE_TAG_START = "<title>";
            const string TITLE_TAG_END = "</title>";

            var titleStartIndex = html.IndexOf(TITLE_TAG_START, StringComparison.OrdinalIgnoreCase);
            var titleEndIndex = html.IndexOf(TITLE_TAG_END, StringComparison.OrdinalIgnoreCase);

            // Check that we have a title and that there isn't already a base tag
            if (titleStartIndex > -1 && titleEndIndex > -1 && html.IndexOf("<base>", StringComparison.OrdinalIgnoreCase) == -1)
            {
                // The index we will insert the base tag
                int index = titleEndIndex + TITLE_TAG_END.Length;

                // Work out how many tab/space characters we need to tab out the generator tag by the same amount as the previous tag
                string tabChars = string.Empty;
                int currentIndex = titleStartIndex - 1;
                while (html[currentIndex] != '\n' && html[currentIndex] != '\r' && (html[currentIndex] == ' ' || html[currentIndex] ==  TAB))
                {
                    tabChars += html[currentIndex];
                    --currentIndex;
                }

                html = html.Substring(0, index)
                    + Environment.NewLine
                    + tabChars
                    + TARGET_TAG
                    + html.Substring(index);
            }

            return html;
        }

        #endregion
    }
}
