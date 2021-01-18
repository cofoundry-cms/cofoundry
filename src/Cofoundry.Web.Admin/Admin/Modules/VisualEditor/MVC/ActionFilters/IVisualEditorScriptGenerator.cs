using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin.Internal
{
    /// <summary>
    /// Used to generate the scripts and CSS links that are added
    /// to a page in order display the visual editor.
    /// </summary>
    public interface IVisualEditorScriptGenerator
    {
        /// <summary>
        /// Generates any scripts or CSS links that need to be
        /// added to the document header.
        /// </summary>
        string CreateHeadScript();

        /// <summary>
        /// Generates any scripts or content that needs to be added
        /// to the end of the document body.
        /// </summary>
        /// <param name="context">
        /// The current ActionContext of the exectuing MVC action.
        /// </param>
        Task<string> CreateBodyScriptAsync(ActionContext context);
    }
}
