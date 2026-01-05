namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{CustomEntityDefinition}"/>.
/// </summary>
public static class CustomEntityDefinitionQueryExtensions
{
    extension(IQueryable<CustomEntityDefinition> customEntityDefinitions)
    {
        public IQueryable<CustomEntityDefinition> FilterByCode(string code)
        {
            var result = customEntityDefinitions
                .Where(i => i.CustomEntityDefinitionCode == code);

            return result;
        }
    }
}
