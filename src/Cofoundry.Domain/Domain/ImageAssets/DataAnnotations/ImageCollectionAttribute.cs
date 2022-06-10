using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// Use this to decorate an integer array of ImageAssetIds and indicate that it should be a collection 
/// of image assets. The editor allows for sorting of linked image assets and you can set filters for restricting image sizes.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ImageCollectionAttribute : Attribute, IMetadataAttribute
{
    /// <summary>
    /// Use this to decorate an integer array of ImageAssetIds and indicate that it should be a collection 
    /// of image assets. The editor allows for sorting of linked image assets and you can set filters for restricting image sizes.
    /// </summary>
    public ImageCollectionAttribute() { }

    /// <summary>
    /// Use this to decorate an integer array of ImageAssetIds and indicate that it should be a collection 
    /// of image assets. The editor allows for sorting of linked image assets and you can set filters for restricting image sizes.
    /// </summary>
    /// <param name="tags">An array of tags for which to filter when browsing for images.</param>
    public ImageCollectionAttribute(params string[] tags)
        : base()
    {
        Tags = tags;
    }

    public void Process(DisplayMetadataProviderContext context)
    {
        MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));

        var modelMetaData = context.DisplayMetadata;

        modelMetaData
            .AddAdditionalValueIfNotEmpty("Tags", Tags)
            .AddAdditionalValueIfNotEmpty("Width", Width)
            .AddAdditionalValueIfNotEmpty("Height", Height)
            .AddAdditionalValueIfNotEmpty("MinWidth", MinWidth)
            .AddAdditionalValueIfNotEmpty("MinHeight", MinHeight);

        modelMetaData.TemplateHint = "ImageAssetCollection";
    }

    public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var ids = propertyInfo.GetValue(model) as ICollection<int>;

        foreach (var id in EnumerableHelper.Enumerate(ids))
        {
            yield return new EntityDependency(ImageAssetEntityDefinition.DefinitionCode, id, false);
        }
    }

    /// <summary>
    /// The width of the image for which to browse.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the image for which to browse.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The minimum width of the image for which to browse.
    /// </summary>
    public int MinWidth { get; set; }

    /// <summary>
    /// The minimum height of the image for which to browse.
    /// </summary>
    public int MinHeight { get; set; }

    /// <summary>
    /// Filters the image selection to only show images with tags that 
    /// match this value.
    /// </summary>
    public string[] Tags { get; set; }
}