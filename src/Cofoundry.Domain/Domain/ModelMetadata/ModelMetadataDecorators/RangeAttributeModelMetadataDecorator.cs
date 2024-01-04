using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web;

public class RangeAttributeModelMetadataDecorator : IModelMetadataDecorator
{
    public bool CanDecorateType(Type type)
    {
        return type == typeof(RangeAttribute);
    }

    public void Decorate(object attribute, DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        if (attribute is not RangeAttribute rangeLengthttribtue)
        {
            throw new ArgumentException("Attribute type is not RangeAttribute", nameof(attribute));
        }

        string? minAttribute;
        string? maxAttribute;

        if (rangeLengthttribtue.OperandType == typeof(string))
        {
            minAttribute = "Minlength";
            maxAttribute = "Maxlength";
        }
        else
        {
            minAttribute = "Min";
            maxAttribute = "Max";
        }

        if (minAttribute != null)
        {
            var modelMetaData = context.DisplayMetadata;

            if (rangeLengthttribtue.Minimum != null)
            {
                modelMetaData.AddAdditionalValueWithValidationMessage(minAttribute, rangeLengthttribtue.Minimum, rangeLengthttribtue);
            }
            if (rangeLengthttribtue.Maximum != null)
            {
                modelMetaData.AddAdditionalValueWithValidationMessage(maxAttribute, rangeLengthttribtue.Maximum, rangeLengthttribtue);
            }
        }

    }
}
