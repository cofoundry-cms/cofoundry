using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PageTemplateSummaryMapper : IPageTemplateSummaryMapper
{
    private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

    public PageTemplateSummaryMapper(
        IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
        )
    {
        _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
    }

    public virtual PageTemplateSummary Map(PageTemplateSummaryQueryModel queryModel)
    {
        var dbPageTemplate = queryModel?.PageTemplate;
        if (dbPageTemplate == null) return null;

        var pageTemplate = new PageTemplateSummary()
        {
            IsArchived = dbPageTemplate.IsArchived,
            Name = dbPageTemplate.Name,
            PageTemplateId = dbPageTemplate.PageTemplateId,
            CreateDate = dbPageTemplate.CreateDate,
            Description = dbPageTemplate.Description,
            FileName = dbPageTemplate.FileName,
            PageType = (PageType)dbPageTemplate.PageTypeId,
            UpdateDate = dbPageTemplate.UpdateDate,
            NumPages = queryModel.NumPages,
            NumRegions = queryModel.NumRegions
        };

        return pageTemplate;
    }
}
