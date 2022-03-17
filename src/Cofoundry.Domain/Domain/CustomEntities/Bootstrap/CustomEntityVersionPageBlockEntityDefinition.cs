namespace Cofoundry.Domain;

/// <summary>
/// Page block data for a specific custom entity version on a custom entity
/// details page.
/// </summary>
public sealed class CustomEntityVersionPageBlockEntityDefinition : IDependableEntityDefinition
{
    public static string DefinitionCode = "COFCEB";

    public string EntityDefinitionCode => DefinitionCode;

    public string Name => "Custom Entity Version Page Block";

    public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
    {
        return new GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(ids);
    }
}
