using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on page module 
    /// views. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid 
    /// poluting the global namespace.
    /// </summary>
    public class CofoundryPageModuleHelper<TModel> 
        : CofoundryPageHelper<TModel> where TModel : IPageModuleDisplayModel
    {
        public CofoundryPageModuleHelper(TModel model)
            : base(model)
        {
            Module = new PageModuleHelper<TModel>();
        }

        /// <summary>
        /// Contains helper functionality relating to the page module
        /// including meta data definitions.
        /// </summary>
        public IPageModuleHelper<TModel> Module { get; set; }
    }
}
