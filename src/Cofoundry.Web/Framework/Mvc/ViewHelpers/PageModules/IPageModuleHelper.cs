using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// This helper exposes page module specific functionality in a page 
    /// module view file.
    /// </summary>
    /// <typeparam name="TViewModel">The module view model</typeparam>
    public interface IPageModuleHelper<TViewModel>
        where TViewModel : IPageModuleDisplayModel
    {
        /// <summary>
        /// Sets a custom display name for the page module. Usually the name is taken 
        /// from the class name e.g. 'RawHtmlDataModel' becomes 'Raw Html', but this 
        /// method allows you to override it e.g. 'Html'
        /// </summary>
        /// <param name="name">The text to use as the display name (max 50 characters)</param>
        IPageModuleHelper<TViewModel> UseDisplayName(string name);

        /// <summary>
        /// Set a custom description for the page module or page module template. This is useful 
        /// to describe the module's functionality to users and help them decide which module or template
        /// to choose when multiple are available.
        /// </summary>
        /// <param name="description">A plain text description about this module or module template</param>
        IPageModuleHelper<TViewModel> UseDescription(string description);
    }
}
