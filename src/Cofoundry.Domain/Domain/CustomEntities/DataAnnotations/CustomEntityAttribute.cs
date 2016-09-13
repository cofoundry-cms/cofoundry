using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate an integer and indicate that it should be the id for a custom entity
    /// of a specific type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomEntityAttribute : RegularExpressionAttribute, IMetadataAttribute, IEntityRelationAttribute
    {
        public CustomEntityAttribute(string customEntityDefinitionCode)
            : base(@"^[1-9]\d*$")
        {
            ErrorMessage = "The {0} field is required";
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.AddAdditionalValueIfNotEmpty("CustomEntityDefinitionCode", CustomEntityDefinitionCode);

            modelMetaData.TemplateHint = "CustomEntitySelector";
        }

        /// <summary>
        /// The code of the custom entity which is allowed to be attached to the collection.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            Condition.Requires(model).IsNotNull();
            Condition.Requires(propertyInfo).IsNotNull();

            var isRequired = !(model is int?);
            var id = (int?)propertyInfo.GetValue(model);

            if (id.HasValue)
            {
                yield return new EntityDependency(CustomEntityDefinitionCode, id.Value, isRequired);
            }
        }
    }
}
