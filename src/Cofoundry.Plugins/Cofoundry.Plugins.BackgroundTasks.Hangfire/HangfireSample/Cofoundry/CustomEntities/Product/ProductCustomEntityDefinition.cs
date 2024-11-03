namespace HangfireSample;

public class ProductCustomEntityDefinition : ICustomEntityDefinition<ProductDataModel>
{
    public const string DefinitionCode = "SPLPRD";

    public string CustomEntityDefinitionCode => DefinitionCode;

    public string Name => "Product";

    public string NamePlural => "Products";

    public string Description => "Example description";

    public bool ForceUrlSlugUniqueness => false;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => true;

    public bool AutoPublish => true;
}
