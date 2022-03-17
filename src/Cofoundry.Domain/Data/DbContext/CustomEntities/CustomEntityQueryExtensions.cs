namespace Cofoundry.Domain.Data;

public static class CustomEntityExtensions
{
    public static IQueryable<CustomEntity> FilterByCustomEntityId(this IQueryable<CustomEntity> customEntities, int customEntityId)
    {
        var result = customEntities
            .Where(i => i.CustomEntityId == customEntityId);

        return result;
    }
}
