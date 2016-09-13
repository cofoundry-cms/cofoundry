using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class CompareAttributeModelMetaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(CompareAttribute)
                || type == typeof(System.Web.Mvc.CompareAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
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
            else if (attribute is System.Web.Mvc.CompareAttribute)
            {
                var compareAttribtue = (System.Web.Mvc.CompareAttribute)attribute;
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
