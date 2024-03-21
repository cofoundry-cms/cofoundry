﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRenderSummaryMapper"/>.
/// </summary>
public class PageVersionSummaryMapper : IPageVersionSummaryMapper
{
    private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
    private readonly IAuditDataMapper _auditDataMapper;

    public PageVersionSummaryMapper(
        IPageTemplateMicroSummaryMapper pageTemplateMapper,
        IAuditDataMapper auditDataMapper
        )
    {
        _pageTemplateMapper = pageTemplateMapper;
        _auditDataMapper = auditDataMapper;
    }

    /// <inheritdoc/>
    public virtual PagedQueryResult<PageVersionSummary> MapVersions(int pageId, PagedQueryResult<PageVersion> dbResult)
    {
        ArgumentNullException.ThrowIfNull(dbResult);
        ArgumentNullException.ThrowIfNull(pageId);

        // We should only check for the latested published version on the first page
        // as it will only be 1st or 2nd in the list (depending on whether there is a draft)
        var hasLatestPublishVersion = dbResult.PageNumber > 1;
        var results = new List<PageVersionSummary>(dbResult.Items.Count);

        foreach (var dbVersion in dbResult.Items.OrderByLatest())
        {
            if (dbVersion.PageId != pageId)
            {
                var msg = $"Invalid PageId. PageVersionSummaryMapper.MapVersions is designed to map versions for a single custom entity only. Excepted PageId {pageId} got {dbVersion.PageId}";
                throw new Exception(msg);
            }

            var result = MapVersion(dbVersion);
            if (!hasLatestPublishVersion && result.WorkFlowStatus == WorkFlowStatus.Published)
            {
                result.IsLatestPublishedVersion = true;
                hasLatestPublishVersion = true;
            }

            results.Add(result);
        }

        return dbResult.ChangeType(results);

    }

    protected PageVersionSummary MapVersion(PageVersion dbPageVersion)
    {
        var result = new PageVersionSummary()
        {
            PageVersionId = dbPageVersion.PageVersionId,
            DisplayVersion = dbPageVersion.DisplayVersion,
            ShowInSiteMap = !dbPageVersion.ExcludeFromSitemap,
            Title = dbPageVersion.Title,
            WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId
        };

        result.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);
        result.AuditData = _auditDataMapper.MapCreateAuditData(dbPageVersion);

        return result;
    }
}
