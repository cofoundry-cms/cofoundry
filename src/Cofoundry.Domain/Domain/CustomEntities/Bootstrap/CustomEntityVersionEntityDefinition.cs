namespace Cofoundry.Domain;

/// <summary>
/// Custom entities can have one or more version, with a collection
/// of versions representing the change history of custom entity
/// data. 
/// </summary>
public sealed class CustomEntityVersionEntityDefinition : IDependableEntityDefinition
{
    public static string DefinitionCode = "COFCEV";

    public string EntityDefinitionCode => DefinitionCode;

    public string Name => "Custom Entity Version";

    public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
    {
        return new GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(ids);
    }
}
