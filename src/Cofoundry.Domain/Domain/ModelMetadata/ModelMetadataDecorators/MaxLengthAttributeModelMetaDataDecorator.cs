﻿using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web;

public class MaxLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
{
    public bool CanDecorateType(Type type)
    {
        return type == typeof(MaxLengthAttribute);
    }

    public void Decorate(object attribute, DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        if (attribute is not MaxLengthAttribute maxLengthttribtue)
        {
            throw new ArgumentException("Attribute type is not MaxLengthAttribute", nameof(attribute));
        }

        var modelMetaData = context.DisplayMetadata;
        modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", maxLengthttribtue.Length, maxLengthttribtue);
    }
}
