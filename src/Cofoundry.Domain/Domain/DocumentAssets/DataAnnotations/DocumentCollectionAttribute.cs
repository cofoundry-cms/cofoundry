using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// This data annotation can be used to decorate a collection of integers, indicating the property represents a set 
/// of document asset ids. The editor allows for sorting and you can set filters for restricting file types.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DocumentCollectionAttribute : Attribute, IMetadataAttribute
{
    public DocumentCollectionAttribute()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentCollectionAttribute"/> class.
    /// </summary>
    /// <param name="tags">An array of tags for which to filter when browsing for document.</param>
    public DocumentCollectionAttribute(params string[] tags)
        : base()
    {
        Tags = tags ?? new string[0];
    }

    public void Process(DisplayMetadataProviderContext context)
    {
        MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));

        var modelMetaData = context.DisplayMetadata;

        DocumentAttributeMetaDataHelper.AddFilterData(modelMetaData, FileExtensions, Tags);

        modelMetaData.TemplateHint = "DocumentAssetCollection";
    }

    public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

        var ids = propertyInfo.GetValue(model) as ICollection<int>;

        foreach (var id in EnumerableHelper.Enumerate(ids))
        {
            yield return new EntityDependency(DocumentAssetEntityDefinition.DefinitionCode, id, false);
        }
    }

    /// <summary>
    /// Filters the document selection to only show documents with these 
    /// file extensions.
    /// </summary>
    public string[] FileExtensions { get; set; }

    /// <summary>
    /// Filters the document selection to only show documents with tags that 
    /// match this value.
    /// </summary>
    public string[] Tags { get; set; }
}