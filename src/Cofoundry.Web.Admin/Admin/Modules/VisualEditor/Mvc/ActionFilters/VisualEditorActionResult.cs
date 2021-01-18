using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// This is used to wrap an existing IActionResult and
    /// inject the scripts and CSS required to run the visual 
    /// editor.
    /// </summary>
    public class VisualEditorActionResult : IActionResult
    {
        private readonly IActionResult _wrappedActionResult;
        private readonly IVisualEditorScriptGenerator _visualEditorScriptGenerator;
        private readonly IHtmlDocumentScriptInjector _htmlDocumentScriptInjector;

        public VisualEditorActionResult(
            IActionResult wrappedActionResult,
            IVisualEditorScriptGenerator visualEditorScriptGenerator,
            IHtmlDocumentScriptInjector htmlDocumentScriptInjector
            )
        {
            _wrappedActionResult = wrappedActionResult;
            _visualEditorScriptGenerator = visualEditorScriptGenerator;
            _htmlDocumentScriptInjector = htmlDocumentScriptInjector;
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
                    var headScript = _visualEditorScriptGenerator.CreateHeadScript();
                    var bodyScript = await _visualEditorScriptGenerator.CreateBodyScriptAsync(context);
                    html = _htmlDocumentScriptInjector.InjectScripts(html, headScript, bodyScript);
                }

                using (var outputStream = new StreamWriter(wrappedStream, Encoding.UTF8))
                {
                    await outputStream.WriteAsync(html);
                    await outputStream.FlushAsync();
                }
            }
        }

        private bool IsHtmlContent(ActionContext context, string html)
        {
            var isTextContent = StringHelper
                .SplitAndTrim(context.HttpContext.Response.ContentType, ';')
                .Contains("text/html");

            return isTextContent && !html.StartsWith("<?xml");
        }
    }
}