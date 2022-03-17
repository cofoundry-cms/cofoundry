using System.Reflection;

namespace Cofoundry.Domain;

public interface IEntityRelationAttribute
{
    IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo);
}
