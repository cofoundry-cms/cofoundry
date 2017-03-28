using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Web
{
    public class MinLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(MinLengthAttribute);
        }

        public void Decorate(object attribute, DisplayMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(MinLengthAttribute));

            var minLengthttribtue = (MinLengthAttribute)attribute;

            modelMetaData.AddAdditionalValueWithValidationMessage("Minlength", minLengthttribtue.Length, minLengthttribtue);
        }
    }
}
