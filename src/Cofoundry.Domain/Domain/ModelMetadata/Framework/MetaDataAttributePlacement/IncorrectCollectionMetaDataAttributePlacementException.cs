﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain;

/// <summary>
/// Thrown when a data model attribute that should be placed on a
/// collection property (e.g. CustomEntityCollectionAttribute) is
/// not placed on an property of the correct type.
/// </summary>
public class IncorrectCollectionMetaDataAttributePlacementException : IncorrectMetaDataAttributePlacementException
{
    /// <summary>
    /// Thrown when a data model attribute that should be placed on a
    /// collection property (e.g. CustomEntityCollectionAttribute) is
    /// not placed on an property of the correct type.
    /// </summary>
    public IncorrectCollectionMetaDataAttributePlacementException(
        Attribute attribute,
        DisplayMetadataProviderContext context,
        IReadOnlyCollection<Type> validParamTypes
        )
        : base(attribute, context, validParamTypes, FormatMessage(attribute, context, validParamTypes))
    {
    }

    /// <summary>
    /// Thrown when a data model attribute that should be placed on a
    /// collection property (e.g. CustomEntityCollectionAttribute) is
    /// not placed on an property of the correct type.
    /// </summary>
    public IncorrectCollectionMetaDataAttributePlacementException(
        Attribute attribute,
        DisplayMetadataProviderContext context,
        IReadOnlyCollection<Type> validParamTypes,
        string message
        )
        : base(attribute, context, validParamTypes, message)
    {
    }

    private static string FormatMessage(
        Attribute attribute,
        DisplayMetadataProviderContext context,
        IReadOnlyCollection<Type> validParamTypes
        )
    {
        var typeNameText = FormatTypeNamesText(validParamTypes);
        var propertyName = FormatPropertyName(context);
        var attributeName = attribute.GetType().Name;
        var firstParamType = validParamTypes.Select(t => t.Name).First();

        return $"{attributeName} can only be placed on properties with a generic collection of {typeNameText} types, e.g. ICollection<{firstParamType}>. Property name is {propertyName} and the type is {context.Key.ModelType}.";
    }
}
