﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// Use this to decorate a collection of INestedDataModel objects, allowing them 
/// to be edited in the admin UI. Optional parameters indicate whether the collection 
/// is sortable.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NestedDataModelCollectionAttribute : ValidateObjectAttribute, IMetadataAttribute, IEntityRelationAttribute
{
    /// <summary>
    /// The minimum number of items that need to be included in the collection. 0 indicates
    /// no minimum.
    /// </summary>
    public int MinItems { get; set; }

    /// <summary>
    /// The maximum number of items that can be included in the collection. 0 indicates
    /// no maximum.
    /// </summary>
    public int MaxItems { get; set; }

    /// <summary>
    /// Set to true to allow the collection ordering to be set by an editor 
    /// using a drag and drop interface. Defaults to false.
    /// </summary>
    public bool IsOrderable { get; set; }

    public void Process(DisplayMetadataProviderContext context)
    {
        string nestedModelTypeName = GetNestedModelTypeName(context);
        var modelMetaData = context.DisplayMetadata;

        modelMetaData
            .AddAdditionalValueIfNotEmpty("MinItems", MinItems)
            .AddAdditionalValueIfNotEmpty("MaxItems", MaxItems)
            .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable)
            .AddAdditionalValueIfNotEmpty("ModelType", nestedModelTypeName);

        modelMetaData.TemplateHint = "NestedDataModelCollection";
    }

    private string GetNestedModelTypeName(DisplayMetadataProviderContext context)
    {
        var singularType = TypeHelper.GetCollectionTypeOrNull(context.Key.ModelType);

        if (singularType == null)
        {
            throw GetIncorrectTypeException(context);
        }

        if (!typeof(INestedDataModel).IsAssignableFrom(singularType))
        {
            throw GetIncorrectTypeException(context);
        }

        var nestedModelName = StringHelper.RemoveSuffix(singularType.Name, "DataModel", StringComparison.OrdinalIgnoreCase);
        return nestedModelName;
    }

    private IncorrectCollectionMetaDataAttributePlacementException GetIncorrectTypeException(DisplayMetadataProviderContext context)
    {
        var propertyName = context.Key.ContainerType?.Name + "." + context.Key.Name;
        var msg = $"{nameof(NestedDataModelCollectionAttribute)} can only be placed on properties with a generic collection of types that inherit from {typeof(INestedDataModel).Name}. Property name is {propertyName} and the type is {context.Key.ModelType}.";
        var exception = new IncorrectCollectionMetaDataAttributePlacementException(this, context, new Type[] { typeof(INestedDataModel) }, msg);

        return exception;
    }

    public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var nestedItems = propertyInfo.GetValue(model) as IEnumerable<INestedDataModel>;

        var dependencies = EnumerableHelper
            .Enumerate(nestedItems)
            .SelectMany(EntityRelationAttributeHelper.GetRelations);

        return dependencies;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var collection = value as IEnumerable<INestedDataModel>;

        if (MinItems > 0 && EnumerableHelper.Enumerate(collection).Count() < MinItems)
        {
            return CreateError(validationContext, $" must have at least {MinItems} items.");
        }

        if (MaxItems > 0 && EnumerableHelper.Enumerate(collection).Count() > MaxItems)
        {
            return CreateError(validationContext, $" cannot have more than {MaxItems} items.");
        }

        return base.IsValid(value, validationContext);
    }

    private ValidationResult CreateError(ValidationContext validationContext, string message)
    {
        string[]? memberNames = string.IsNullOrEmpty(validationContext.MemberName) ? null : [validationContext.MemberName];
        return new ValidationResult(validationContext.MemberName + message, memberNames);
    }
}
