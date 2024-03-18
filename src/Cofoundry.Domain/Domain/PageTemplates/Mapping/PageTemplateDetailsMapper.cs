﻿using Cofoundry.Domain.Data;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageTemplateDetailsMapper"/>.
/// </summary>
public class PageTemplateDetailsMapper : IPageTemplateDetailsMapper
{
    /// <inheritdoc/>
    public virtual PageTemplateDetails? Map(PageTemplateDetailsQueryModel? queryModel)
    {
        var dbPageTemplate = queryModel?.PageTemplate;
        if (dbPageTemplate == null || queryModel == null)
        {
            return null;
        }

        var pageTemplate = new PageTemplateDetails()
        {
            FullPath = dbPageTemplate.FullPath,
            IsArchived = dbPageTemplate.IsArchived,
            Name = dbPageTemplate.Name,
            PageTemplateId = dbPageTemplate.PageTemplateId,
            CreateDate = dbPageTemplate.CreateDate,
            Description = dbPageTemplate.Description,
            FileName = dbPageTemplate.FileName,
            PageType = (PageType)dbPageTemplate.PageTypeId,
            CustomEntityModelType = dbPageTemplate.CustomEntityModelType,
            UpdateDate = dbPageTemplate.UpdateDate,
            NumPages = queryModel.NumPages,
            CustomEntityDefinition = queryModel.CustomEntityDefinition
        };

        pageTemplate.Regions = queryModel
            .PageTemplate
            .PageTemplateRegions
            .Select(MapRegion)
            .ToArray();

        return pageTemplate;
    }

    protected static PageTemplateRegionDetails MapRegion(PageTemplateRegion dbRegion)
    {
        return new PageTemplateRegionDetails()
        {
            CreateDate = dbRegion.CreateDate,
            IsCustomEntityRegion = dbRegion.IsCustomEntityRegion,
            Name = dbRegion.Name,
            PageTemplateId = dbRegion.PageTemplateId,
            PageTemplateRegionId = dbRegion.PageTemplateRegionId,
            UpdateDate = dbRegion.UpdateDate
        };
    }
}
