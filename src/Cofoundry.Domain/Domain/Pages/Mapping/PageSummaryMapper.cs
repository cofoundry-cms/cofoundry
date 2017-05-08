using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Common mapping functionality for PageSummaries
    /// </summary>
    internal class PageSummaryMapper
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public PageSummaryMapper(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Finishes off bulk mapping of tags and page routes in a PageSummary object
        /// </summary>
        public async Task MapAsync(IEnumerable<PageSummary> pages)
        {
            var routes = await _queryExecutor.GetAllAsync<PageRoute>();

            var ids = pages
                .Select(p => p.PageId)
                .ToArray();

            var pageTags = _dbContext
                .PageTags
                .AsNoTracking()
                .Where(p => ids.Contains(p.PageId))
                .Select(t => new
                {
                    PageId = t.PageId,
                    Tag = t.Tag.TagText
                })
                .ToList();

            foreach (var page in pages)
            {
                var pageRoute = routes.SingleOrDefault(r => r.PageId == page.PageId);
                EntityNotFoundException.ThrowIfNull(pageRoute, page.PageId);

                Mapper.Map(pageRoute, page);

                page.Tags = pageTags
                    .Where(t => t.PageId == page.PageId)
                    .Select(t => t.Tag)
                    .ToArray();
            }
        }
    }
}
