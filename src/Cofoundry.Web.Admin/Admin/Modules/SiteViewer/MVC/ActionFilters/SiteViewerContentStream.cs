﻿using Cofoundry.Domain;
using Newtonsoft.Json;
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
    /// Designed only for use with SiteViewerContentFilterAttribute. 
    /// </remarks>
    internal class SiteViewerContentStream : MemoryStream
    {
        #region constructor

        const char TAB = '\t';

        private readonly StringBuilder _outputString = new StringBuilder();
        private readonly Stream _outputStream = null;
        private readonly IPageResponseData _pageResponseData;

        public SiteViewerContentStream(
            Stream outputStream,
            IPageResponseData pageResponseData
            )
        {
            _outputStream = outputStream;
            _pageResponseData = pageResponseData;
        }

        #endregion

        #region implementation

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Write out the entire stream to memory before manipulating it.
            _outputString.Append(Encoding.UTF8.GetString(buffer));
        }

        public override void Close()
        {
            string html = _outputString.ToString();

            // Check for not XML
            if (!html.StartsWith("<?xml"))
            {
                html = AddCofoundryJavascript(html);
            }

            // Write the string back to the response
            byte[] rawResult = Encoding.UTF8.GetBytes(html);
            _outputStream.Write(rawResult, 0, rawResult.Length);

            base.Close();
            _outputStream.Close();
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

            // TODO: something with the data
            // If _pageResponseData is null then it's a static page
            // else its a pagey page
            if (_pageResponseData != null)
            {
                var responseJson = JsonConvert.SerializeObject(_pageResponseData);
                // DO we also need the user data so we can work out permissions?
                // ICurrentUserViewHelper is what we provide in the angular helper, would need to inject that in
            }

            return html;
        }

        #endregion
    }
}
