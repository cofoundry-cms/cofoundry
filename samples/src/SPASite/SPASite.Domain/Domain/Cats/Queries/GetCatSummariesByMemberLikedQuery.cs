namespace SPASite.Domain;

public class GetCatSummariesByMemberLikedQuery : IQuery<IReadOnlyCollection<CatSummary>>
{
    public GetCatSummariesByMemberLikedQuery() { }

    public GetCatSummariesByMemberLikedQuery(int id)
    {
        UserId = id;
    }

    public int UserId { get; set; }
}
