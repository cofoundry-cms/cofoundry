using Cofoundry.Domain;
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
    /// <remarks>
    /// The default service is used when the admin package is not installed
    /// and simply returns an empty result.
    /// </remarks>
    public class DefaultVisualEditorStateService : IVisualEditorStateService
    {
        private VisualEditorState _visualEditorStateCache = null;

        public Task<VisualEditorState> GetCurrentAsync()
        {
            if (_visualEditorStateCache == null)
            {
                _visualEditorStateCache = new VisualEditorState();
            }

            return Task.FromResult(_visualEditorStateCache);
        }
    }
}
