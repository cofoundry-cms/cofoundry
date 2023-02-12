namespace Cofoundry.Domain.Internal;

public interface IPageModelExtensionConfigurationRepository
{
    IReadOnlyCollection<ExtensionRegistrationOptions> GetByTemplateId(int pageTemplateId);
}
