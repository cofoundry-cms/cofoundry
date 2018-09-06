using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate an integer id collection property that links to a set of entities. The entity
    /// must have a definition that implements IDependableEntityDefinition.  Defining relations allow the system to
    /// detect and prevent entities used in required fields from being removed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityDependencyCollectionAttribute : Attribute, IEntityRelationAttribute, IMetadataAttribute
    {
        #region constructors

        public EntityDependencyCollectionAttribute(string entityDefinitionCode)
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

            var ids = propertyInfo.GetValue(model) as ICollection<int>;

            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(EntityDefinitionCode, id, false);
            }
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));
        }

        #endregion

        #region properties

        public string EntityDefinitionCode { get; set; }

        #endregion
    }
}
