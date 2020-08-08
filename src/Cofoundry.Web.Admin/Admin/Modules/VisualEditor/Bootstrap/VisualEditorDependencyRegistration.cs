using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin.DependencyRegistration
{
    public class VisualEditorDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.Register<IVisualEditorActionResultFactory, VisualEditorActionResultFactory>();
            container.RegisterAll<IVisualEditorRequestExclusionRule>();

            var overrideOptions = RegistrationOptions.Override(RegistrationOverridePriority.Low);
            container.Register<IVisualEditorStateService, AdminVisualEditorStateService>(overrideOptions);
        }
    }
}