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
    public class RangeAttributeModelMetaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(RangeAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
        {
            Condition.Requires(attribute).IsNotNull();
            Condition.Requires(attribute).IsOfType(typeof(RangeAttribute));

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
