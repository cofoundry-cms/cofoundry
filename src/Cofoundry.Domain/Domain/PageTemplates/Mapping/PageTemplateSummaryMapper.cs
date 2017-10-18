using Cofoundry.Domain.Data;
using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to PageTemplateSummary objects.
    /// </summary>
    public class PageTemplateSummaryMapper : IPageTemplateSummaryMapper
    {
        private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public PageTemplateSummaryMapper(
            IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
            )
        {
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        }

        /// <summary>
        /// Maps an EF PageTemplate record from the db into an PageTemplateSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbPageTemplate">Query model with data from the database.</param>
        public PageTemplateSummary Map(PageTemplateSummaryQueryModel queryModel)
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
}
