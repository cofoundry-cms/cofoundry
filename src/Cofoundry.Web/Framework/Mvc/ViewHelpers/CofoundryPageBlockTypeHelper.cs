using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on page block type 
    /// views. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid 
    /// poluting the global namespace.
    /// </summary>
    public class CofoundryPageBlockTypeHelper<TModel> 
        : CofoundryPageHelper<TModel>, ICofoundryBlockTypeHelper<TModel>
        where TModel : IPageBlockTypeDisplayModel
    {
        public CofoundryPageBlockTypeHelper(
            IContentRouteLibrary contentRouteLibrary,
            IStaticFileViewHelper staticFileViewHelper,
            ISettingsViewHelper settings,
            ICurrentUserViewHelper currentUser,
            IJavascriptViewHelper js,
            IHtmlSanitizerHelper sanitizer,
            ICofoundryHtmlHelper html
            )
            : base(
                contentRouteLibrary,
                staticFileViewHelper,
                settings,
                currentUser,
                js,
                sanitizer,
                html
                )
        {
            BlockType = new PageBlockHelper<TModel>();
        }

        /// <summary>
        /// Contains helper functionality relating to the page block type
        /// including meta data definitions.
        /// </summary>
        public IPageBlockHelper<TModel> BlockType { get; set; }
    }
}
