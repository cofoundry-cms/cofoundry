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
        : CofoundryPageHelper<TModel> //where TModel : IPageBlockTypeDisplayModel
    {
        public CofoundryPageBlockTypeHelper(ViewContext viewContext, TModel model)
            : base(viewContext, model)
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
