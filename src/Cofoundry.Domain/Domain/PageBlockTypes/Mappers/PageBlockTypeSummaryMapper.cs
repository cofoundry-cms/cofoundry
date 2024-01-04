using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageBlockTypeSummaryMapper"/>.
/// </summary>
public class PageBlockTypeSummaryMapper : IPageBlockTypeSummaryMapper
{
    /// <inheritdoc/>
    public PageBlockTypeSummary Map(PageBlockType dbPageBlockType)
    {
        var result = new PageBlockTypeSummary()
        {
            Description = dbPageBlockType.Description,
            FileName = dbPageBlockType.FileName,
            Name = dbPageBlockType.Name,
            PageBlockTypeId = dbPageBlockType.PageBlockTypeId
        };

        result.Templates = dbPageBlockType
            .PageBlockTemplates
            .Select(Map)
            .ToArray();

        return result;
    }

    private PageBlockTypeTemplateSummary Map(PageBlockTypeTemplate dbTemplate)
    {
        var result = new PageBlockTypeTemplateSummary()
        {
            FileName = dbTemplate.FileName,
            Name = dbTemplate.Name,
            PageBlockTypeTemplateId = dbTemplate.PageBlockTypeTemplateId
        };

        return result;
    }
}
