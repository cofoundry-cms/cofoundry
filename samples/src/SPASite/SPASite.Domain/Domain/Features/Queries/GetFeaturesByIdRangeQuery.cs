namespace SPASite.Domain;

public class GetFeaturesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, Feature>>
{
    public GetFeaturesByIdRangeQuery()
    {
        FeatureIds = new List<int>();
    }

    public GetFeaturesByIdRangeQuery(IReadOnlyCollection<int> ids)
    {
        FeatureIds = ids;
    }

    public IReadOnlyCollection<int> FeatureIds { get; set; }
}
