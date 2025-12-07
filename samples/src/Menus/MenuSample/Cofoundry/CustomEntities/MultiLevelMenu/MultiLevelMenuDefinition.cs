namespace MenuSample;

/// <summary>
/// The multi-level menu example demonstrates how to create
/// a menu with an unlimited number of menu levels (i.e a tree 
/// structure) using a recursive data model definition.
/// </summary>
public class MultiLevelMenuDefinition
    : ICustomEntityDefinition<MultiLevelMenuDataModel>
    , ICustomizedTermCustomEntityDefinition
{
    public const string DefinitionCode = "MNUMUL";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Multi-level Menu";

    public string NamePlural => "Multi-level Menus";

    public string Description => "A completely customizable menu consisting of a unlimited levels of menu items.";

    public bool ForceUrlSlugUniqueness => true;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => true;

    public bool AutoPublish => false;

    /// <summary>
    /// Here we customize the title of the menu to be displayed
    /// as 'Identifier', which better describes its purpose.
    /// </summary>
    public Dictionary<string, string> CustomTerms => new()
    {
        { CustomizableCustomEntityTermKeys.Title, "Identifier" }
    };
}
