namespace SPASite.Domain;

public class GetFeaturesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, Feature>>
{
    public GetFeaturesByIdRangeQuery()
    {
        FeatureIds = [];
    }

    public GetFeaturesByIdRangeQuery(IReadOnlyCollection<int> ids)
    {
        FeatureIds = ids;
    }

    public IReadOnlyCollection<int> FeatureIds { get; set; }
}
