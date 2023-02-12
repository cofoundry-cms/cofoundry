namespace Cofoundry.Domain.Internal;

public class PageModelExtensionConfigurationRepository : IPageModelExtensionConfigurationRepository
{
    private readonly IReadOnlyDictionary<int, IReadOnlyDictionary<Type, ExtensionRegistrationOptions>> _registrationsByTemplateFileName;

    public PageModelExtensionConfigurationRepository(
        IEnumerable<IPageExtensionRegistration> pageExtensionRegistrations
        )
    {
        _registrationsByTemplateFileName = ProcessRegistrations(pageExtensionRegistrations);
    }

    private static IReadOnlyDictionary<int, IReadOnlyDictionary<Type, ExtensionRegistrationOptions>> ProcessRegistrations(
        IEnumerable<IPageExtensionRegistration> pageExtensionRegistrations
        )
    {
        var options = new ExtensionRegistrationContext("Page");
        foreach (var registration in pageExtensionRegistrations)
        {
            registration.RegisterPageExtensions(options);
        }

        var baseRegistrations = options.GetRegistrations();
        // TODO: copy/configure for page templates

        return new int[] { 1 }.ToDictionary(k => k, v => baseRegistrations);
    }

    public IReadOnlyCollection<ExtensionRegistrationOptions> GetByTemplateId(int pageTemplateId)
    {
        // TODO: Implement defs per template
        //return _registrationsByTemplateFileName.GetValueOrDefault(pageTemplateId);
        return _registrationsByTemplateFileName
            .Single()
            .Value
            .Values
            .ToArray();
    }
}
