using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Bootstrap
{
    public class PageControllerImplementationDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterPerRequestScope<INotFoundViewHelper, NotFoundViewHelper>()
                .RegisterType<IPageLocaleParser, PageLocaleParser>()
                .RegisterType<ISiteViewerActionFactory, DefaultSiteViewerActionFactory>()
                .RegisterType<ISetupPageActionFactory, DefaultSetupPageActionFactory>()
                .RegisterType<IPageActionRoutingStepFactory, PageActionRoutingStepFactory>()

                .RegisterType<ICheckSiteIsSetupRoutingStep, CheckSiteIsSetupRoutingStep>()
                .RegisterType<IInitStateRoutingStep, InitStateRoutingStep>()
                .RegisterType<ITryFindPageRoutingInfoRoutingStep, TryFindPageRoutingInfoRoutingStep>()
                .RegisterType<IValidateEntityEditModeRoutingStep, ValidateEntityEditModeRoutingStep>()
                .RegisterType<IValidateDraftVersionRoutingStep, ValidateDraftVersionRoutingStep>()
                .RegisterType<IValidateSpecificVersionRoutingRoutingStep, ValidateSpecificVersionRoutingRoutingStep>()
                .RegisterType<IGetNotFoundRouteRoutingStep, GetNotFoundRouteRoutingStep>()
                .RegisterType<IShowSiteViewerRoutingStep, ShowSiteViewerRoutingStep>()
                .RegisterType<IGetPageRenderDataRoutingStep, GetPageRenderDataRoutingStep>()
                .RegisterType<ISetCachePolicyRoutingStep, SetCachePolicyRoutingStep>()
                .RegisterType<IGetFinalResultRoutingStep, GetFinalResultRoutingStep>()
                ;
        }
    }
}