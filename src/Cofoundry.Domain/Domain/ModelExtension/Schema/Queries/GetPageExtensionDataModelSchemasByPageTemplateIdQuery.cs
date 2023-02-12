namespace Cofoundry.Domain;

public class GetPageExtensionDataModelSchemasByPageTemplateIdQuery : IQuery<ICollection<EntityExtensionDataModelSchema>>
{
    public GetPageExtensionDataModelSchemasByPageTemplateIdQuery()
    {
    }

    public GetPageExtensionDataModelSchemasByPageTemplateIdQuery(int pageTemplateId)
    {
        PageTemplateId = pageTemplateId;
    }

    public int PageTemplateId { get; set; }
}
