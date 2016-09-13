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
    public class MaxLengthAttributeModelMetaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(MaxLengthAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(MaxLengthAttribute));

            var maxLengthttribtue = (MaxLengthAttribute)attribute;

            modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", maxLengthttribtue.Length, maxLengthttribtue);
        }
    }
}
