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
    public class RangeAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(RangeAttribute);
        }

        public void Decorate(object attribute, DisplayMetadataProviderContext context)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            if (!(attribute is RangeAttribute))
            {
                throw new ArgumentException("Attribute type is not RangeAttribute", nameof(attribute));
            }

            var rangeLengthttribtue = (RangeAttribute)attribute;

            string minAttribute = null;
            string maxAttribute = null;

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
}
