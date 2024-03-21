using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain;

/// <summary>
/// Indicates the property of a model that can be used as a
/// description field. Typically this is used in a grid of items 
/// to describe the item.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PreviewDescriptionAttribute : Attribute, IMetadataAttribute
{
    public void Process(DisplayMetadataProviderContext context)
    {
        context
            .DisplayMetadata
            .AdditionalValues
            .Add("PreviewDescription", true)
            ;
    }
}
