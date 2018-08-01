using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Service for extracting and validating the visual editor state from
    /// the http request.
    /// </summary>
    public interface IVisualEditorStateService
    {
        /// <summary>
        /// Gets the current visual editor state, which
        /// is typically cached for the liftime of the request.
        /// </summary>
        Task<VisualEditorState> GetCurrentAsync();
    }
}
