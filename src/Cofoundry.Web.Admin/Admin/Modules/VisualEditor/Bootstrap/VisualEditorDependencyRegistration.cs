using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container.Register<IVisualEditorActionResultFactory, VisualEditorActionResultFactory>();
            container.RegisterAll<IVisualEditorRequestExclusionRule>();
        }
    }
}