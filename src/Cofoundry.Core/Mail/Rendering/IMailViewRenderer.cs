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
        /// Renders the view file to a string.
        /// </summary>
        /// <param name="viewPath">Relative path to the view to render.</param>
        /// <returns>View file rendered to a string.</returns>
        Task<string> RenderAsync(string viewPath);

        /// <summary>
        /// Renders the view file to a string with the specified view model.
        /// </summary>
        /// <typeparam name="TModel">View model type.</typeparam>
        /// <param name="viewPath">Relative path to the view to render.</param>
        /// <param name="model">The model to pass to the view to render.</param>
        /// <returns>View file rendered to a string.</returns>
        Task<string> RenderAsync<TModel>(string viewPath, TModel model);
    }
}
