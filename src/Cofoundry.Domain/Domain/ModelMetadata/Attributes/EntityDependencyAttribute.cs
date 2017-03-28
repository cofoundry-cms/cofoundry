using System;
using System.Collections.Generic;
using System.Reflection;
using Conditions;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate an integer id property that links to another entity. The entity
    /// must have a definition that implements IDependableEntityDefinition.  Defining relations allow the system to
    /// detect and prevent entities used in required fields from being removed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityDependencyAttribute : Attribute, IEntityRelationAttribute
    {
        #region constructors

        public EntityDependencyAttribute(string entityDefinitionCode)
        {
            Condition.Requires(entityDefinitionCode)
                .IsNotNull()
                .HasLength(6);

            EntityDefinitionCode = entityDefinitionCode;
        }

        #endregion

        #region interface implementation

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            Condition.Requires(model).IsNotNull();
            Condition.Requires(propertyInfo).IsNotNull();

            var isRequired = !(model is int?);
            var id = (int?)propertyInfo.GetValue(model);

            if (id.HasValue)
            {
                yield return new EntityDependency(EntityDefinitionCode, id.Value, isRequired);
            }
        }

        #endregion

        #region properties

        public string EntityDefinitionCode { get; set; }

        #endregion
    }
}