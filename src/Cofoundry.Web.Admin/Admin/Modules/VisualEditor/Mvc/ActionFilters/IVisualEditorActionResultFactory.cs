using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// Factory for generating the VisualEditorActionResult
    /// that wraps the current action current AxctionResult
    /// and modified it to include visual editor scripts.
    /// </summary>
    public interface IVisualEditorActionResultFactory
    {
        IActionResult Create(IActionResult wrappedActionResult);
    }
}