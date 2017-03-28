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
    public class MaxLengthAttributeModelMetadataDecorator : IModelMetadataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(MaxLengthAttribute);
        }

        public void Decorate(object attribute, DisplayMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(MaxLengthAttribute));

            var maxLengthttribtue = (MaxLengthAttribute)attribute;

            modelMetaData.AddAdditionalValueWithValidationMessage("Maxlength", maxLengthttribtue.Length, maxLengthttribtue);
        }
    }
}
