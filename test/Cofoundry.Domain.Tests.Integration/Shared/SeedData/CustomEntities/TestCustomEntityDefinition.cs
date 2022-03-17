namespace Cofoundry.Domain.Tests.Integration;

public class TestCustomEntityDefinition : ICustomEntityDefinition<TestCustomEntityDataModel>
{
    public const string Code = "TSTENT";
    public const string EntityName = "Test Entity";

    public string CustomEntityDefinitionCode => Code;

    public string Name => EntityName;

    public string NamePlural => "Test Entities";

    public string Description => "Test description";

    public bool ForceUrlSlugUniqueness => false;

    public bool HasLocale => false;

    public bool AutoGenerateUrlSlug => false;

    public bool AutoPublish => false;
}
