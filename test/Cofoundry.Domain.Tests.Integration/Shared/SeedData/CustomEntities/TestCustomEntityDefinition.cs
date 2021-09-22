namespace Cofoundry.Domain.Tests.Integration
{
    public class TestCustomEntityDefinition : ICustomEntityDefinition<TestCustomEntityDataModel>
    {
        public const string DefinitionCode = "TSTENT";

        public string CustomEntityDefinitionCode => DefinitionCode;

        public string Name => "Test Entity";

        public string NamePlural => "Test Entities";

        public string Description => "Test description";

        public bool ForceUrlSlugUniqueness => false;

        public bool HasLocale => false;

        public bool AutoGenerateUrlSlug => false;

        public bool AutoPublish => false;
    }
}
