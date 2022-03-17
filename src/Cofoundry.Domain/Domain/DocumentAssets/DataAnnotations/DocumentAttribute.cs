using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// Use with an (optionally nullable) integer to indicate this is for the id of 
/// a DocumentAsset. A non-null integer indicates this is a required field. Optional
/// parameters allow you to restricts the file extensions permitted to be selected.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DocumentAttribute : RegularExpressionAttribute, IMetadataAttribute
{
    public DocumentAttribute()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentAttribute"/> class.
    /// </summary>
    /// <param name="tags">An array of tags for which to filter when browsing for this document</param>
    public DocumentAttribute(params string[] tags)
        : base(@"^[1-9]\d*$")
    {
        ErrorMessage = "The {0} field is required";
        Tags = tags ?? new string[0];
    }

    public void Process(DisplayMetadataProviderContext context)
    {
        MetaDataAttributePlacementValidator.ValidatePropertyType(this, context, typeof(int), typeof(int?));

        var modelMetaData = context.DisplayMetadata;

        DocumentAttributeMetaDataHelper.AddFilterData(modelMetaData, FileExtensions, Tags);

        modelMetaData.TemplateHint = "DocumentAsset";
    }

    public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

        var isRequired = !(model is int?);
        var id = (int?)propertyInfo.GetValue(model);

        if (id.HasValue)
        {
            yield return new EntityDependency(DocumentAssetEntityDefinition.DefinitionCode, id.Value, isRequired);
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
    public string[] Tags { get; private set; }
}