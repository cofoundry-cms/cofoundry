namespace Cofoundry.Domain;

public class ExtensionRegistrationOptions
{
    public ExtensionRegistrationOptions(Type type)
    {
        Type = type;
    }

    public Type Type { get; }

    /// <summary>
    /// i.e. prop name, no spaces
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Name of the section in the admin panel
    /// </summary>
    public string GroupName { get; set; }

    public EntityExtensionLoadProfile LoadProfile { get; set; }
}
