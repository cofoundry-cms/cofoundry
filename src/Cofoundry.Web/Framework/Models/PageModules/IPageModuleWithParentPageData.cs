using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Optional interface you can use to decorate an instance of
    /// IModuleDisplayData to add some contextual information about the
    /// page the module is parented to.
    /// </summary>
    public interface IPageModuleWithParentPageData : IPageModuleDisplayModel
    {
        /// <summary>
        /// Provides access to the page containing the module. 
        /// </summary>
        IEditablePageViewModel ParentPage { get; set; }
    }
}
