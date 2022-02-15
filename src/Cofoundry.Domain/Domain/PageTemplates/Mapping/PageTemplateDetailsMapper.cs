using Cofoundry.Domain.Data;
using Cofoundry.Domain.QueryModels;
using System.Linq;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class PageTemplateDetailsMapper : IPageTemplateDetailsMapper
    {
        public virtual PageTemplateDetails Map(PageTemplateDetailsQueryModel queryModel)
        {
            var dbPageTemplate = queryModel?.PageTemplate;
            if (dbPageTemplate == null) return null;

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
                .ToList();

            return pageTemplate;
        }

        protected PageTemplateRegionDetails MapRegion(PageTemplateRegion dbRegion)
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
}
