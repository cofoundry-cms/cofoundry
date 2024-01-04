using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageTemplateDetailsMapper"/>.
/// </summary>
public class PageTemplateSummaryMapper : IPageTemplateSummaryMapper
{
    /// <inheritdoc/>
    public virtual PageTemplateSummary? Map(PageTemplateSummaryQueryModel? queryModel)
    {
        var dbPageTemplate = queryModel?.PageTemplate;
        if (dbPageTemplate == null || queryModel == null)
        {
            return null;
        }

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
