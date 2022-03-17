namespace Cofoundry.Domain;

public sealed class PageVersionBlockEntityDefinition : IDependableEntityDefinition
{
    public const string DefinitionCode = "COFPGB";

    public string EntityDefinitionCode => DefinitionCode;

    public string Name => "Page Version Block";

    public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
    {
        return new GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(ids);
    }
}
