using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageTemplateMicroSummaryMapper"/>.
/// </summary>
public class PageTemplateMicroSummaryMapper : IPageTemplateMicroSummaryMapper
{
    private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

    public PageTemplateMicroSummaryMapper(
        IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
        )
    {
        _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbPageTemplate))]
    public virtual PageTemplateMicroSummary? Map(PageTemplate? dbPageTemplate)
    {
        if (dbPageTemplate == null)
        {
            return null;
        }

        var pageTemplate = new PageTemplateMicroSummary
        {
            CustomEntityDefinitionCode = dbPageTemplate.CustomEntityDefinitionCode,
            FullPath = dbPageTemplate.FullPath,
            IsArchived = dbPageTemplate.IsArchived,
            Name = dbPageTemplate.Name,
            PageTemplateId = dbPageTemplate.PageTemplateId
        };

        pageTemplate.CustomEntityModelType = _pageTemplateCustomEntityTypeMapper.Map(dbPageTemplate.CustomEntityModelType);

        return pageTemplate;
    }
}
