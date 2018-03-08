using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a Metadata attribute that can be parsed by 
    /// a MetadataProvider.
    /// </summary>
    public interface IMetadataAttribute
    {
        /// <summary>
        /// Implement this to customize the model metadata, modifying
        /// context.DisplayMetaData or adding extra data to the 
        /// context.DisplayMetaData.AdditionalValues collection. 
        /// </summary>
        void Process(DisplayMetadataProviderContext context);
    }
}
