using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// This can be used to decorate an integer id collection property that links to a set of entities. The entity
/// must have a definition that implements IDependableEntityDefinition.  Defining relations allow the system to
/// detect and prevent entities used in required fields from being removed.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EntityDependencyCollectionAttribute : Attribute, IEntityRelationAttribute, IMetadataAttribute
{
    public EntityDependencyCollectionAttribute(string entityDefinitionCode)
    {
        ArgumentNullException.ThrowIfNull(entityDefinitionCode);
        if (entityDefinitionCode.Length != 6)
        {
            throw new ArgumentException(nameof(entityDefinitionCode) + " must be 6 characters in length.", nameof(entityDefinitionCode));
        }

        EntityDefinitionCode = entityDefinitionCode;
    }

    public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(propertyInfo);

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

    public string EntityDefinitionCode { get; set; }
}