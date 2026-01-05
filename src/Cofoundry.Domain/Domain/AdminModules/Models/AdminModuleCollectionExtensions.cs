namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for collections of <see cref="AdminModule"/>.
/// </summary>
public static class AdminModuleCollectionExtensions
{
    extension(IEnumerable<AdminModule> source)
    {
        public IEnumerable<AdminModule> SetStandardOrdering()
        {
            return source
                .OrderBy(r => r.MenuCategory)
                .ThenBy(r => r.PrimaryOrdering)
                .ThenByDescending(r => r.SecondaryOrdering)
                .ThenBy(r => r.Title);
        }
    }
}
