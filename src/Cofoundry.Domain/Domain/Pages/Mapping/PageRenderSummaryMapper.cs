using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class PageRenderSummaryMapper : IPageRenderSummaryMapper
    {
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
        private readonly IOpenGraphDataMapper _openGraphDataMapper;

        public PageRenderSummaryMapper(
            IPageTemplateMicroSummaryMapper pageTemplateMapper,
            IOpenGraphDataMapper openGraphDataMapper
            )
        {
            _pageTemplateMapper = pageTemplateMapper;
            _openGraphDataMapper = openGraphDataMapper;
        }

        /// <summary>
        /// Genric mapper for objects that inherit from PageRenderSummary.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
        /// <param name="pageRoute">The page route to map to the new object.</param>
        public virtual T Map<T>(PageVersion dbPageVersion, PageRoute pageRoute)
            where T : PageRenderSummary, new()
        {
            if (dbPageVersion == null) throw new ArgumentNullException(nameof(dbPageVersion));
            if (pageRoute == null) throw new ArgumentNullException(nameof(pageRoute));

            var page = MapInternal<T>(dbPageVersion);
            page.PageRoute = pageRoute;

            return page;
        }

        /// <summary>
        /// Genric mapper for objects that inherit from PageRenderSummary.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
        /// <param name="pageRouteLookup">Dictionary containing all page routes.</param>
        public virtual T Map<T>(PageVersion dbPageVersion, IDictionary<int, PageRoute> pageRouteLookup)
            where T : PageRenderSummary, new()
        {
            if (dbPageVersion == null) throw new ArgumentNullException(nameof(dbPageVersion));
            if (pageRouteLookup == null) throw new ArgumentNullException(nameof(pageRouteLookup));

            var page = MapInternal<T>(dbPageVersion);

            page.PageRoute = pageRouteLookup.GetOrDefault(page.PageId);

            if (page.PageRoute == null)
            {
                throw new Exception($"Unable to locate a page route when mapping a {nameof(PageRenderSummary)} with an id of {page.PageId}.");
            }

            return page;
        }

        protected T MapInternal<T>(PageVersion dbPageVersion) where T : PageRenderSummary, new()
        {
            var page = new T()
            {
                MetaDescription = dbPageVersion.MetaDescription,
                PageId = dbPageVersion.PageId,
                PageVersionId = dbPageVersion.PageVersionId,
                Title = dbPageVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId,
                CreateDate = dbPageVersion.CreateDate
            };

            page.OpenGraph = _openGraphDataMapper.Map(dbPageVersion);
            return page;
        }
    }
}
