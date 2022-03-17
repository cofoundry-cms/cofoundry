namespace Cofoundry.Domain;

public class GetPageTemplateDetailsByIdQuery : IQuery<PageTemplateDetails>
{
    public GetPageTemplateDetailsByIdQuery()
    {
    }

    public GetPageTemplateDetailsByIdQuery(int pageTemplateId)
    {
        PageTemplateId = pageTemplateId;
    }

    public int PageTemplateId { get; set; }
}
