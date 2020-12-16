using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// <para>
    /// A IPageModuleDisplayModelMapper class handles the mapping from
    /// a display model to a data model.
    /// </para>
    /// <para>
    /// The mapper supports DI which gives you flexibility in what data
    /// you want to include in the display model and how you want to 
    /// map it. Mapping is done in batch to improve performance when 
    /// the same block type is used multiple times on a page.
    /// </para>
    /// </summary>
    public class PageListDisplayModelMapper : IPageBlockTypeDisplayModelMapper<PageListDataModel>
    {
        private readonly IContentRepository _contentRepository;

        public PageListDisplayModelMapper(
            IContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<PageListDataModel> context,
            PageBlockTypeDisplayModelMapperResult<PageListDataModel> result
            )
        {
            var allPageIds = context.Items.SelectManyDistinctModelValues(d => d.PageIds);

            // Page routes are cached and so are the quickest way to get simple page information.
            // If we needed more data we could use a different but slower query to get it.
            var query = new GetPageRenderSummariesByIdRangeQuery(allPageIds, context.PublishStatusQuery);
            var allPageRoutes = await _contentRepository
                .WithExecutionContext(context.ExecutionContext)
                .Pages()
                .GetByIdRange(allPageIds)
                .AsRenderSummaries(context.PublishStatusQuery)
                .ExecuteAsync();

            foreach (var item in context.Items)
            {
                var mapped = new PageListDisplayModel();

                // Here will get the relevant pages and order them correctly.
                mapped.Pages = allPageRoutes
                    .FilterAndOrderByKeys(item.DataModel.PageIds)
                    .ToList();

                result.Add(item, mapped);
            }
        }
    }
}