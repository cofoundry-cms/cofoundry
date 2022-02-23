namespace Cofoundry.Domain
{
    /// <summary>
    /// A document asset is a non-image file that has been uploaded to the 
    /// CMS. The name could be misleading here as any file type except
    /// images are supported, but at least it is less ambigous than the 
    /// term 'file'.
    /// </summary>
    public sealed class DocumentAssetEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFDOC";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Document"; } }
    }
}