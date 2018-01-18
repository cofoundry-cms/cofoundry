using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// A IPageModuleDisplayModelMapper class handles the mapping from
    /// a display model to a data model.
    /// 
    /// The mapper supports DI which gives you flexibility in what data
    /// you want to include in the display model and how you want to 
    /// map it. Mapping is done in batch to improve performance when 
    /// the same block type is used multiple times on a page.
    /// </summary>
    public class PageListDisplayModelMapper : IPageBlockTypeDisplayModelMapper<PageListDataModel>
    {
        private readonly IPageRepository _pageRepository;

        public PageListDisplayModelMapper(
            IPageRepository pageRepository
            )
        {
            _pageRepository = pageRepository;
        }

        public async Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(
            IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<PageListDataModel>> inputCollection,
            PublishStatusQuery publishStatus
            )
        {
            var allPageIds = inputCollection.SelectManyDistinctModelValues(d => d.PageIds);

            // Page routes are cached and so are the quickest way to get simple page information.
            // If we needed more data we could use a different but slower query to get it.
            var query = new GetPageRenderDetailsByIdRangeQuery(allPageIds);
            var allPageRoutes = await _pageRepository.GetPageRenderDetailsByIdRangeAsync(query);

            var results = new List<PageBlockTypeDisplayModelMapperOutput>(inputCollection.Count);
            foreach (var input in inputCollection)
            {
                var output = new PageListDisplayModel();

                // Here will get the relevant pages and order them correctly. 
                // Additionally if we are viewing the published version of the page
                // then we make sure we only show published pages in the list.

                output.Pages = allPageRoutes.ToFilteredAndOrderedCollection(input.DataModel.PageIds);

                // The CreateOutput() method wraps the mapped display 
                // model with it's identifier so we can identify later on
                results.Add(input.CreateOutput(output));
            }

            return results;
        }
    }
}