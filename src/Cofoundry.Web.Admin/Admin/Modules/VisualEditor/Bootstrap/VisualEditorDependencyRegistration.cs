using Cofoundry.Core.DependencyInjection;
using Cofoundry.Web.Admin.Internal;

namespace Cofoundry.Web.Admin.Registration;

public class VisualEditorDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IVisualEditorScriptGenerator, VisualEditorScriptGenerator>()
            .Register<IHtmlDocumentScriptInjector, HtmlDocumentScriptInjector>()
            .Register<IVisualEditorActionResultFactory, VisualEditorActionResultFactory>()
            .RegisterAll<IVisualEditorRequestExclusionRule>();

        var overrideOptions = RegistrationOptions.Override(RegistrationOverridePriority.Low);
        container.Register<IVisualEditorStateService, AdminVisualEditorStateService>(overrideOptions);
    }
}
