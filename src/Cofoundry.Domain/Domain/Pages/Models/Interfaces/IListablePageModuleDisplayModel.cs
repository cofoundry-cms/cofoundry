using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Optional interface you can use to decorate an instance of
    /// IModuleDisplayData to add some contextual information to your module display
    /// view model.
    /// </summary>
    public interface IListablePageModuleDisplayModel : IPageModuleDisplayModel
    {
        /// <summary>
        /// Gives information about the collection that a module is in.
        /// </summary>
        ListablePageModuleRenderContext ListContext { get; set; }
    }
}
