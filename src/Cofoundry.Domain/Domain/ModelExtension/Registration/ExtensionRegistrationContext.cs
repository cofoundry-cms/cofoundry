namespace Cofoundry.Domain;

public class ExtensionRegistrationContext
{
    private Dictionary<Type, ExtensionRegistrationOptions> _registrations = new Dictionary<Type, ExtensionRegistrationOptions>();
    private readonly string _entityName;

    public ExtensionRegistrationContext(string entityName)
    {
        _entityName = entityName;
    }

    public ExtensionRegistrationContext Add<TModel>(Action<ExtensionRegistrationOptions> optionsConfiguration = null) where TModel : IEntityExtensionDataModel
    {
        if (_registrations.ContainsKey(typeof(TModel)))
        {
            throw new Exception("TODO");
        }

        var type = typeof(TModel);
        var typeName = StringHelper.RemoveSuffix(type.Name, "DataModel");
        typeName = StringHelper.RemovePrefix(typeName, _entityName);

        if (string.IsNullOrWhiteSpace(typeName))
        {
            typeName = type.Name;
        }

        var options = new ExtensionRegistrationOptions(type)
        {
            Name = TextFormatter.Camelize(typeName),
            GroupName = TextFormatter.PascalCaseToSentence(typeName),
            LoadProfile = EntityExtensionLoadProfile.Details,
        };

        optionsConfiguration?.Invoke(options);
        options.Name = TextFormatter.Camelize(typeName);
        _registrations.Add(options.Type, options);

        return this;
    }

    public IReadOnlyDictionary<Type, ExtensionRegistrationOptions> GetRegistrations()
    {
        return _registrations;
    }
}

