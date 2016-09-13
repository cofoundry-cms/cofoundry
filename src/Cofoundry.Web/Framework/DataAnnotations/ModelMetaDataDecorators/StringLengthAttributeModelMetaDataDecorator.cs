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
    public class StringLengthAttributeModelMetaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(StringLengthAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(StringLengthAttribute));

            var stringLengthttribtue = (StringLengthAttribute)attribute;

            modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", stringLengthttribtue.MaximumLength, stringLengthttribtue);

            if (stringLengthttribtue.MinimumLength > 0)
            {
                modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", stringLengthttribtue.MinimumLength, stringLengthttribtue);
            }
        }
    }
}
