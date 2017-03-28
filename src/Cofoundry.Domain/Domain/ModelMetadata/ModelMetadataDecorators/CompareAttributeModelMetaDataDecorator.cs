using Conditions;
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
    public class CompareAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(CompareAttribute);
        }

        public void Decorate(object attribute, DisplayMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            string otherProperty;
            ValidationAttribute valAttribute;

            if (attribute is CompareAttribute)
            {
                var compareAttribtue = (CompareAttribute)attribute;
                otherProperty = compareAttribtue.OtherProperty;
                valAttribute = compareAttribtue;
            }
            else
            {
                throw new ArgumentException("attribute type is not a CompareAttribute", "attribute");
            }

            modelMetaData.AddAdditionalValueWithValidationMessage("Match", otherProperty, valAttribute);
        }
    }
}
