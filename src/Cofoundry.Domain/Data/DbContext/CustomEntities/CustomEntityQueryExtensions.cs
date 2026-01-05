namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{CustomEntity}"/>.
/// </summary>
public static class CustomEntityExtensions
{
    extension(IQueryable<CustomEntity> customEntities)
    {
        public IQueryable<CustomEntity> FilterByCustomEntityId(int customEntityId)
        {
            var result = customEntities.Where(i => i.CustomEntityId == customEntityId);

            return result;
        }
    }
}
