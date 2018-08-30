using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate an integer id property that links to another entity. The entity
    /// must have a definition that implements IDependableEntityDefinition.  Defining relations allow the system to
    /// detect and prevent entities used in required fields from being removed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityDependencyAttribute : Attribute, IEntityRelationAttribute, IMetadataAttribute
    {
        #region constructors

        public EntityDependencyAttribute(string entityDefinitionCode)
        {
            if (entityDefinitionCode == null) throw new ArgumentNullException(nameof(entityDefinitionCode));
            if (entityDefinitionCode.Length != 6)
            {
                throw new ArgumentException(nameof(entityDefinitionCode) + " must be 6 characters in length.", nameof(entityDefinitionCode));
            }

            EntityDefinitionCode = entityDefinitionCode;
        }

        #endregion

        #region interface implementation

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var isRequired = !(model is int?);
            var id = (int?)propertyInfo.GetValue(model);

            if (id.HasValue)
            {
                yield return new EntityDependency(EntityDefinitionCode, id.Value, isRequired);
            }
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidatePropertyType(this, context, typeof(int), typeof(int?));
        }

        #endregion

        #region properties

        public string EntityDefinitionCode { get; set; }

        #endregion
    }
}