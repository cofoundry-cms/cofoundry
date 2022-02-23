namespace Cofoundry.Domain
{
    public sealed class SettingsEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFSET";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Settings"; } }
    }
}