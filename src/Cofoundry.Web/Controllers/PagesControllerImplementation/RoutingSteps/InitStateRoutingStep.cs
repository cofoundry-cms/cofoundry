using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initialises the key parameters of the PageActionRoutingState
    /// object e.g. the UserContext and SiteViewerMode properties
    /// </summary>
    public class InitStateRoutingStep : IInitStateRoutingStep
    {
        private readonly IUserContextService _userContextService;
        private readonly ContentSettings _contentSettings;

        public InitStateRoutingStep(
            IUserContextService userContextService,
            ContentSettings contentSettings
            )
        {
            _userContextService = userContextService;
            _contentSettings = contentSettings;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            state.UserContext = _userContextService.GetCurrentContext();

            // Work out whether to view the page in live/draft/edit mode.
            // We use live by default (for logged out users) or for authenticated
            // users we can show draft too.
            var siteViewerMode = SiteViewerMode.Live;
            if (state.UserContext.IsCofoundryUser())
            {
                if (state.InputParameters.VersionId.HasValue)
                {
                    siteViewerMode = SiteViewerMode.SpecificVersion;
                }
                else if (!Enum.TryParse(state.InputParameters.SiteViewerMode, true, out siteViewerMode))
                {
                    siteViewerMode = SiteViewerMode.Any;
                }
            }
            else if (_contentSettings.AlwaysShowUnpublishedPages)
            {
                // We can optionally set the siteviewer mode to any - ie show draft and published pages
                // This is used in scenarios where devs are making modifications against a live db using a 
                // local debug version of the site but aren't ready to publish the pages yet.
                siteViewerMode = SiteViewerMode.Any;
            }
            state.SiteViewerMode = siteViewerMode;

            return Task.FromResult(true);
        }
    }
}
