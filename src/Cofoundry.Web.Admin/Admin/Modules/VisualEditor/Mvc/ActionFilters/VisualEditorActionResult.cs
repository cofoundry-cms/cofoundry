using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.Json;
using Newtonsoft.Json;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorActionResult : IActionResult
    {
        const char TAB = '\t';
        private readonly IActionResult _wrappedActionResult;
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly IPageResponseDataCache _pageResponseDataCache;
        private readonly IRazorViewRenderer _razorViewRenderer;
        private readonly IResourceLocator _resourceLocator;

        public VisualEditorActionResult(
            IActionResult wrappedActionResult,
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            IAdminRouteLibrary adminRouteLibrary,
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            IPageResponseDataCache pageResponseDataCache,
            IRazorViewRenderer razorViewRenderer,
            IResourceLocator resourceLocator
            )
        {
            _wrappedActionResult = wrappedActionResult;
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _adminRouteLibrary = adminRouteLibrary;
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _pageResponseDataCache = pageResponseDataCache;
            _razorViewRenderer = razorViewRenderer;
            _resourceLocator = resourceLocator;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var wrappedStream = context.HttpContext.Response.Body;

            using (var stream = new MemoryStream())
            {
                context.HttpContext.Response.Body = stream;
                string html = null;

                try
                {
                    await _wrappedActionResult.ExecuteResultAsync(context);

                    stream.Seek(0, SeekOrigin.Begin);
                    html = Encoding.UTF8.GetString(stream.ToArray()).Trim();
                }
                finally
                {
                    context.HttpContext.Response.Body = wrappedStream;
                }

                // Check for not XML
                if (IsHtmlContent(context, html))
                {
                    html = await AddCofoundryDependenciesAsync(html, context);
                }

                using (var outputStream = new StreamWriter(wrappedStream, Encoding.UTF8))
                {
                    await outputStream.WriteAsync(html);
                }
            }
        }

        #region helpers

        private bool IsHtmlContent(ActionContext context, string html)
        {
            var isTextContent = StringHelper
                .SplitAndTrim(context.HttpContext.Response.ContentType, ';')
                .Contains("text/html");

            return isTextContent && !html.StartsWith("<?xml");
        }

        private async Task<string> AddCofoundryDependenciesAsync(string html, ActionContext context)
        {
            const string HEAD_TAG_END = "</head>";
            const string BODY_TAG_END = "</body>";

            var insertHeadIndex = html.IndexOf(HEAD_TAG_END, StringComparison.OrdinalIgnoreCase) - 1;

            if (insertHeadIndex > 0)
            {
                html = html.Substring(0, insertHeadIndex)
                    + Environment.NewLine + TAB
                    + _staticResourceReferenceRenderer.ScriptTag(_adminRouteLibrary.VisualEditor, "contentpage").ToString() + TAB
                    + _staticResourceReferenceRenderer.CssTag(_adminRouteLibrary.VisualEditor, "site-viewer").ToString()
                    + html.Substring(insertHeadIndex);
            }

            var insertBodyIndex = html.IndexOf(BODY_TAG_END, StringComparison.OrdinalIgnoreCase) - 1;

            if (insertBodyIndex > 0)
            {
                var pageResponseData = _pageResponseDataCache.Get();

                // When using IPageBlockWithParentPageData and referencing the parent page we get a
                // Self referencing loop error. Rather than set this globally we ignore this specifically here
                var settings = _jsonSerializerSettingsFactory.Create();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                var responseJson = JsonConvert.SerializeObject(pageResponseData, settings);

                var toolbarHtml = await _razorViewRenderer.RenderViewAsync(context, _adminRouteLibrary.VisualEditor.VisualEditorToolbarViewPath(), pageResponseData);
                var svgIcons = await RenderSvgIconsToStringAsync();

                html = html.Substring(0, insertBodyIndex)
                    + Environment.NewLine + TAB
                    + "<script>var Cofoundry = { 'PageResponseData': " + responseJson + " }</script>" + TAB
                    + toolbarHtml
                    + string.Format("<!-- SVG ICONS --><div style='{0}'>{1}</div><!-- END SVG ICONS -->", "display:none", svgIcons)
                    + html.Substring(insertBodyIndex);
            }

            return html;
        }

        private async Task<string> RenderSvgIconsToStringAsync()
        {
            var virtualFile = _resourceLocator.GetFile(_adminRouteLibrary.VisualEditor.StaticResource("svg-cache.html"));
            string result = null;

            using (var stream = virtualFile.CreateReadStream())
            {
                var reader = new StreamReader(stream);
                result = await reader.ReadToEndAsync();
            }

            return result;
        }

        #endregion
    }
}