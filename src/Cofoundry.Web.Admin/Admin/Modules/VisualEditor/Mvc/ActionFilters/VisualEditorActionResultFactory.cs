using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// Factory for generating the VisualEditorActionResult
    /// that wraps the current action current AxctionResult
    /// and modified it to include visual editor scripts.
    /// </summary>
    public class VisualEditorActionResultFactory : IVisualEditorActionResultFactory
    {
        private readonly IVisualEditorScriptGenerator _visualEditorScriptGenerator;
        private readonly IHtmlDocumentScriptInjector _htmlDocumentScriptInjector;

        public VisualEditorActionResultFactory(
            IVisualEditorScriptGenerator visualEditorScriptGenerator,
            IHtmlDocumentScriptInjector htmlDocumentScriptInjector
            )
        {
            _visualEditorScriptGenerator = visualEditorScriptGenerator;
            _htmlDocumentScriptInjector = htmlDocumentScriptInjector;
        }

        public IActionResult Create(IActionResult wrappedActionResult)
        {
            return new VisualEditorActionResult(
                wrappedActionResult,
                _visualEditorScriptGenerator,
                _htmlDocumentScriptInjector
                );
        }
    }
}