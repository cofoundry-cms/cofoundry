using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Bootstrap
{
    public class PageControllerImplementationDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<INotFoundViewHelper, NotFoundViewHelper>()
                .RegisterType<IPageLocaleParser, PageLocaleParser>()
                .RegisterType<ISetupPageActionFactory, DefaultSetupPageActionFactory>()
                .RegisterType<IPageActionRoutingStepFactory, PageActionRoutingStepFactory>()

                .RegisterType<ICheckSiteIsSetupRoutingStep, CheckSiteIsSetupRoutingStep>()
                .RegisterType<IInitStateRoutingStep, InitStateRoutingStep>()
                .RegisterType<ITryFindPageRoutingInfoRoutingStep, TryFindPageRoutingInfoRoutingStep>()
                .RegisterType<IValidateEntityEditModeRoutingStep, ValidateEntityEditModeRoutingStep>()
                .RegisterType<IValidateEditPermissionsRoutingStep, ValidateEditPermissionsRoutingStep>()
                .RegisterType<IValidateDraftVersionRoutingStep, ValidateDraftVersionRoutingStep>()
                .RegisterType<IValidateSpecificVersionRoutingRoutingStep, ValidateSpecificVersionRoutingRoutingStep>()
                .RegisterType<IGetNotFoundRouteRoutingStep, GetNotFoundRouteRoutingStep>()
                .RegisterType<IGetPageRenderDataRoutingStep, GetPageRenderDataRoutingStep>()
                .RegisterType<ISetCachePolicyRoutingStep, SetCachePolicyRoutingStep>()
                .RegisterType<IGetFinalResultRoutingStep, GetFinalResultRoutingStep>()

                .RegisterType<IPageResponseDataCache, PageResponseDataCache>()
                ;
        }
    }
}