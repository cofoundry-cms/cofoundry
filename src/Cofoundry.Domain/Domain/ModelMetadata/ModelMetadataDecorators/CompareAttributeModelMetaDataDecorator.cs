using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web;

public class CompareAttributeModelMetadataDecorator : IModelMetadataDecorator
{
    public bool CanDecorateType(Type type)
    {
        return type == typeof(CompareAttribute);
    }

    public void Decorate(object attribute, DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        if (attribute is not CompareAttribute compareAttribtue)
        {
            throw new ArgumentException("Attribute type is not CompareAttribute", nameof(attribute));
        }


        var otherProperty = compareAttribtue.OtherProperty;
        var valAttribute = compareAttribtue;

        var modelMetaData = context.DisplayMetadata;
        modelMetaData.AddAdditionalValueWithValidationMessage("Match", otherProperty, valAttribute);
    }
}
