using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Shared code for extracting EntityDependencies
    /// from models with properties that might use IEntityRelationAttribute
    /// to defined dependencies.
    /// </summary>
    public static class EntityRelationAttributeHelper
    {
        /// <summary>
        /// Enumerates the properies in a model and extracts dependencies
        /// from properties marked with an attribute that inherits from 
        /// IEntityRelationAttribute.
        /// </summary>
        /// <param name="model">Model to extract relations from.</param>
        public static IEnumerable<EntityDependency> GetRelations(object model)
        {
            var relationProperties = model
                .GetType()
                .GetTypeInfo()
                .GetProperties()
                .Select(p => new {
                    Property = p,
                    Attribute = p.GetCustomAttributes(false)
                        .Where(a => a is IEntityRelationAttribute)
                        .FirstOrDefault()
                })
                .Where(p => p.Attribute != null);

            foreach (var relationProperty in relationProperties)
            {
                var attribtue = (IEntityRelationAttribute)relationProperty.Attribute;
                foreach (var relation in attribtue.GetRelations(model, relationProperty.Property))
                {
                    yield return relation;
                }
            }
        }
    }
}
