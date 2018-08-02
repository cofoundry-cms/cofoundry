using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageRenderDetailsMapper : IPageRenderDetailsMapper
    {
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
        private readonly IOpenGraphDataMapper _openGraphDataMapper;

        public PageRenderDetailsMapper(
            IPageTemplateMicroSummaryMapper pageTemplateMapper,
            IOpenGraphDataMapper openGraphDataMapper
            )
        {
            _pageTemplateMapper = pageTemplateMapper;
            _openGraphDataMapper = openGraphDataMapper;
        }

        /// <summary>
        /// Maps the basic properties on a PageRenderDetails.
        /// </summary>
        /// <remarks>
        /// This isn't a very fully featured map function and will likey
        /// be reworked later on.
        /// </remarks>
        public PageRenderDetails Map(
            PageVersion dbPageVersion
            )
        {
            var page = new PageRenderDetails()
            {
                MetaDescription = dbPageVersion.MetaDescription,
                PageId = dbPageVersion.PageId,
                PageVersionId = dbPageVersion.PageVersionId,
                Title = dbPageVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId,
                CreateDate = dbPageVersion.CreateDate
            };

            page.OpenGraph = _openGraphDataMapper.Map(dbPageVersion);
            page.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);

            page.Regions = dbPageVersion
                .PageTemplate
                .PageTemplateRegions
                .Select(r => new PageRegionRenderDetails()
                {
                    PageTemplateRegionId = r.PageTemplateRegionId,
                    Name = r.Name
                    // Blocks mapped elsewhere
                })
                .ToList();

            return page;
        }
    }
}
