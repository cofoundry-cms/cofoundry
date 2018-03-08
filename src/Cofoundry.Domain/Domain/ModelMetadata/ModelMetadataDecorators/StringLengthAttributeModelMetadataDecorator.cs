using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web
{
    public class StringLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(StringLengthAttribute);
        }

        public void Decorate(object attribute, DisplayMetadataProviderContext context)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            if (!(attribute is StringLengthAttribute))
            {
                throw new ArgumentException("Attribute type is not StringLengthAttribute", nameof(attribute));
            }

            var stringLengthttribtue = (StringLengthAttribute)attribute;

            var modelMetaData = context.DisplayMetadata;
            modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", stringLengthttribtue.MaximumLength, stringLengthttribtue);

            if (stringLengthttribtue.MinimumLength > 0)
            {
                modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", stringLengthttribtue.MinimumLength, stringLengthttribtue);
            }
        }
    }
}
