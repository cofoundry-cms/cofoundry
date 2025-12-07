namespace MenuSample;

/// <summary>
/// <para>
/// The nested menu demonstrates how you can build menus
/// with a pre-defined number of menu levels. This is achieved
/// using nested data models and teh [NestedDataModelCollection]
/// attribute.
/// </para>
/// <para>
/// This example only contains one nested menu level,
/// but you could define more by creating and nested more menu 
/// types. To an indeterminate number of menu levels (i.e. a tree 
/// structure) have a look at the MultiLevelMenuDataModel.
/// </para>
/// </summary>
public class NestedMenuDefinition
    : ICustomEntityDefinition<NestedMenuDataModel>
    , ICustomizedTermCustomEntityDefinition
{
    public const string DefinitionCode = "MNUNST";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Nested Menu";

    public string NamePlural => "Nested Menus";

    public string Description => "A two-level menu consisting of a menu items that can have child menu items.";

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
