using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a Metadata attribute that can be parsed by 
    /// a MetadataProvider
    /// </summary>
    public interface IMetadataAttribute
    {
        /// <summary>
        /// Implement this to customize the model metadata, adding
        /// any items to the AdditionalValues. 
        /// </summary>
        void Process(DisplayMetadata modelMetaData);
    }
}
