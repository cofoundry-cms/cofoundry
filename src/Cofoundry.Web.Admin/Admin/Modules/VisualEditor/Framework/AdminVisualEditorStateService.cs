using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Service for extracting and validating the visual editor state from
    /// the http request.
    /// </summary>
    public class AdminVisualEditorStateService : IVisualEditorStateService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserContextService _userContextService;
        private readonly ContentSettings _contentSettings;
        private IVisualEditorStateCache _visualEditorStateCache;
        private AdminSettings _adminSettings;

        public AdminVisualEditorStateService(
            IHttpContextAccessor httpContextAccessor,
            IUserContextService userContextService,
            ContentSettings contentSettings,
            IVisualEditorStateCache visualEditorStateCache,
            AdminSettings adminSettings
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userContextService = userContextService;
            _contentSettings = contentSettings;
            _visualEditorStateCache = visualEditorStateCache;
            _adminSettings = adminSettings;
        }

        public async Task<VisualEditorState> GetCurrentAsync()
        {
            var state = _visualEditorStateCache.Get();

            // cache set across a single request, so no locking required
            if (state == null)
            {
                state = await CreateAsync();
                _visualEditorStateCache.Set(state);
            }

            return state;
        }

        private async Task<VisualEditorState> CreateAsync()
        {
            var state = new VisualEditorState();
            if (_adminSettings.Disabled) return state;

            var requestParameters = GetRequestParameters();

            var cofoundryUserContext = await _userContextService.GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.AreaCode);

            // Work out whether to view the page in live/draft/edit mode.
            // We use live by default (for logged out users) or for authenticated
            // users we can show draft too.
            var visualEditorMode = VisualEditorMode.Live;
            if (cofoundryUserContext.IsCofoundryUser())
            {
                if (requestParameters.VersionId.HasValue)
                {
                    visualEditorMode = VisualEditorMode.SpecificVersion;
                }
                else if (!Enum.TryParse(requestParameters.VisualEditorMode, true, out visualEditorMode))
                {
                    visualEditorMode = VisualEditorMode.Any;
                }
            }
            else if (_contentSettings.AlwaysShowUnpublishedData)
            {
                // We can optionally set the visual editor mode to any - ie show draft and published pages
                // This is used in scenarios where devs are making modifications against a live db using a 
                // local debug version of the site but aren't ready to publish the pages yet.
                visualEditorMode = VisualEditorMode.Any;
            }

            return new VisualEditorState(visualEditorMode);
        }

        private class VisualEditorRequestParameters
        {
            public string VisualEditorMode { get; set; }

            public int? VersionId { get; set; }
        }

        private VisualEditorRequestParameters GetRequestParameters()
        {
            var requestParameters = new VisualEditorRequestParameters();

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request != null)
            {
                requestParameters.VisualEditorMode = request.Query["mode"];
                requestParameters.VersionId = IntParser.ParseOrNull(request.Query["version"]);
            }

            return requestParameters;
        }
    }
}
