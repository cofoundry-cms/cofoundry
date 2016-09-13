using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class MinLengthAttributeModelMetaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(MinLengthAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(MinLengthAttribute));

            var minLengthttribtue = (MinLengthAttribute)attribute;

            modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", minLengthttribtue.Length, minLengthttribtue);
        }
    }
}
