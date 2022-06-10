using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web;

public class MinLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
{
    public bool CanDecorateType(Type type)
    {
        return type == typeof(MinLengthAttribute);
    }

    public void Decorate(object attribute, DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        if (!(attribute is MinLengthAttribute))
        {
            throw new ArgumentException("Attribute type is not MinLengthAttribute", nameof(attribute));
        }

        var minLengthttribtue = (MinLengthAttribute)attribute;

        var modelMetaData = context.DisplayMetadata;
        modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", minLengthttribtue.Length, minLengthttribtue);
    }
}
