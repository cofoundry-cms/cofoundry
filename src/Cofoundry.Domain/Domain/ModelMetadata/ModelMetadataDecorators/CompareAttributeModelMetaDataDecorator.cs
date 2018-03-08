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

        public void Decorate(object attribute, DisplayMetadataProviderContext context)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            if (!(attribute is CompareAttribute))
            {
                throw new ArgumentException("Attribute type is not CompareAttribute", nameof(attribute));
            }


            var compareAttribtue = (CompareAttribute)attribute;
            var otherProperty = compareAttribtue.OtherProperty;
            var valAttribute = compareAttribtue;

            var modelMetaData = context.DisplayMetadata;
            modelMetaData.AddAdditionalValueWithValidationMessage("Match", otherProperty, valAttribute);
        }
    }
}
