using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Used to render mail template view files.
    /// </summary>
    public interface IMailViewRenderer
    {
        /// <summary>
        /// Renders the view file to a string, returning null if the
        /// view file could not be found.
        /// </summary>
        /// <param name="viewPath">Relative path to the view to render.</param>
        /// <returns>View file rendered to a string if found; otherwise null.</returns>
        Task<string> RenderAsync(string viewPath);

        /// <summary>
        /// Renders the view file to a string with the specified view 
        /// model, returning null if the view file could not be found.
        /// </summary>
        /// <typeparam name="TModel">View model type.</typeparam>
        /// <param name="viewPath">Relative path to the view to render.</param>
        /// <param name="model">Relative path to the view to render.</param>
        /// <returns>View file rendered to a string if found; otherwise null.</returns>
        Task<string> RenderAsync<TModel>(string viewPath, TModel model);
    }
}
