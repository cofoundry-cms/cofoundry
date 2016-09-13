using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate an integer id array property that links to a set of entities. The entity
    /// must have a definition that implements IDependableEntityDefinition.  Defining relations allow the system to
    /// detect and prevent entities used in required fields from being removed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityDependencyCollectionAttribute : Attribute, IEntityRelationAttribute
    {
        #region constructors

        public EntityDependencyCollectionAttribute(string entityDefinitionCode)
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

            var ids = propertyInfo.GetValue(model) as int[];

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    yield return new EntityDependency(EntityDefinitionCode, id, false);
                }
            }
        }

        #endregion

        #region properties

        public string EntityDefinitionCode { get; set; }

        #endregion
    }
}
