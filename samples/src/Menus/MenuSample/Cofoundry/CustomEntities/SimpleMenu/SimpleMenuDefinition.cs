namespace MenuSample;

/// <summary>
/// The simple menu demonstrates how you can build a content
/// manageable list of pages using the [PageCollection] attribute.
/// </summary>
public class SimpleMenuDefinition
    : ICustomEntityDefinition<SimpleMenuDataModel>
    , ICustomizedTermCustomEntityDefinition
    , IOrderableCustomEntityDefinition
{
    public const string DefinitionCode = "MNUSPL";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Simple Menu";

    public string NamePlural => "Simple Menus";

    public string Description => "A simple customizable menu consisting of a single selection of pages.";

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

    public CustomEntityOrdering Ordering => CustomEntityOrdering.Partial;
}
