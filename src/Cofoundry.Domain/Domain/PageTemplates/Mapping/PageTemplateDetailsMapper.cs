using Cofoundry.Domain.Data;
using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageTemplateDetails objects.
    /// </summary>
    public class PageTemplateDetailsMapper : IPageTemplateDetailsMapper
    {
        /// <summary>
        /// Maps an EF PageTemplate record from the db into an PageTemplateDetailsMapper 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="queryModel">Query data returned from the database.</param>
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
                CreateDate = DbDateTimeMapper.AsUtc(dbPageTemplate.CreateDate),
                Description = dbPageTemplate.Description,
                FileName = dbPageTemplate.FileName,
                PageType = (PageType)dbPageTemplate.PageTypeId,
                CustomEntityModelType = dbPageTemplate.CustomEntityModelType,
                UpdateDate = DbDateTimeMapper.AsUtc(dbPageTemplate.UpdateDate),
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
                CreateDate = DbDateTimeMapper.AsUtc(dbRegion.CreateDate),
                IsCustomEntityRegion = dbRegion.IsCustomEntityRegion,
                Name = dbRegion.Name,
                PageTemplateId = dbRegion.PageTemplateId,
                PageTemplateRegionId = dbRegion.PageTemplateRegionId,
                UpdateDate = DbDateTimeMapper.AsUtc(dbRegion.UpdateDate)
            };
        }
    }
}
