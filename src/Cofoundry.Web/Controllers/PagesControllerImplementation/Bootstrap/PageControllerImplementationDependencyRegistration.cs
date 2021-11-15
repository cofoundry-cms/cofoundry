using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Registration
{
    public class PageControllerImplementationDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<INotFoundViewHelper, NotFoundViewHelper>()
                .Register<IPageLocaleParser, PageLocaleParser>()
                .Register<ISetupPageActionFactory, DefaultSetupPageActionFactory>()
                .Register<IPageActionRoutingStepFactory, PageActionRoutingStepFactory>()

                .Register<ICheckSiteIsSetupRoutingStep, CheckSiteIsSetupRoutingStep>()
                .Register<IInitVisualEditorStateRoutingStep, InitVisualEditorStateRoutingStep>()
                .Register<IInitUserContextRoutingStep, InitUserContextRoutingStep>()
                .Register<ITryFindPageRoutingInfoRoutingStep, TryFindPageRoutingInfoRoutingStep>()
                .Register<IValidateEntityEditModeRoutingStep, ValidateEntityEditModeRoutingStep>()
                .Register<IValidateEditPermissionsRoutingStep, ValidateEditPermissionsRoutingStep>()
                .Register<IValidateDraftVersionRoutingStep, ValidateDraftVersionRoutingStep>()
                .Register<IValidateSpecificVersionRoutingRoutingStep, ValidateSpecificVersionRoutingRoutingStep>()
                .Register<IValidateAccessRulesRoutingStep, ValidateAccessRulesRoutingStep>()
                .Register<IGetNotFoundRouteRoutingStep, GetNotFoundRouteRoutingStep>()
                .Register<IGetPageRenderDataRoutingStep, GetPageRenderDataRoutingStep>()
                .Register<ISetCachePolicyRoutingStep, SetCachePolicyRoutingStep>()
                .Register<IGetFinalResultRoutingStep, GetFinalResultRoutingStep>()

                .Register<IPageResponseDataCache, PageResponseDataCache>()
                ;
        }
    }
}