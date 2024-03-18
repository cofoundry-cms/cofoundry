﻿using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web;

public class StringLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
{
    public bool CanDecorateType(Type type)
    {
        return type == typeof(StringLengthAttribute);
    }

    public void Decorate(object attribute, DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        if (attribute is not StringLengthAttribute stringLengthttribtue)
        {
            throw new ArgumentException("Attribute type is not StringLengthAttribute", nameof(attribute));
        }

        var modelMetaData = context.DisplayMetadata;
        modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", stringLengthttribtue.MaximumLength, stringLengthttribtue);

        if (stringLengthttribtue.MinimumLength > 0)
        {
            modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", stringLengthttribtue.MinimumLength, stringLengthttribtue);
        }
    }
}
