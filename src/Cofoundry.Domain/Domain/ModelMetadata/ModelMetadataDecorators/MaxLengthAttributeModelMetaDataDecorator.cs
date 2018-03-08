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
    public class MaxLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(MaxLengthAttribute);
        }

        public void Decorate(object attribute, DisplayMetadataProviderContext context)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            if (!(attribute is MaxLengthAttribute))
            {
                throw new ArgumentException("Attribute type is not MaxLengthAttribute", nameof(attribute));
            }

            var maxLengthttribtue = (MaxLengthAttribute)attribute;

            var modelMetaData = context.DisplayMetadata;
            modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", maxLengthttribtue.Length, maxLengthttribtue);
        }
    }
}
