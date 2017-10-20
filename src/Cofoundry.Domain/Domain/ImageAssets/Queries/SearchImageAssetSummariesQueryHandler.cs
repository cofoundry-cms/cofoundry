using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class SearchImageAssetSummariesQueryHandler 
        : IAsyncQueryHandler<SearchImageAssetSummariesQuery, PagedQueryResult<ImageAssetSummary>>
        , IPermissionRestrictedQueryHandler<SearchImageAssetSummariesQuery, PagedQueryResult<ImageAssetSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetSummaryMapper _imageAssetSummaryMapper;

        public SearchImageAssetSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetSummaryMapper imageAssetSummaryMapper
            )
        {
            _dbContext = dbContext;
            _imageAssetSummaryMapper = imageAssetSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<ImageAssetSummary>> ExecuteAsync(SearchImageAssetSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .ImageAssets
                .AsNoTracking()
                .Where(i => !i.IsDeleted);

            // Filter by tags
            if (!string.IsNullOrEmpty(query.Tags))
            {
                var tags = TagParser.Split(query.Tags).ToList();
                foreach (string tag in tags)
                {
                    // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                    string localTag = tag;

                    dbQuery = dbQuery.Where(p => p.ImageAssetTags
                                                  .Select(t => t.Tag.TagText)
                                                  .Contains(localTag)
                                           );
                }
            }

            // Filter Dimensions
            if (query.Height > 0)
            {
                dbQuery = dbQuery.Where(p => p.Height == query.Height);
            }
            else if (query.MinHeight > 0)
            {
                dbQuery = dbQuery.Where(p => p.Height >= query.MinHeight);
            }

            if (query.Width > 0)
            {
                dbQuery = dbQuery.Where(p => p.Width == query.Width);
            }
            else if (query.MinWidth > 0)
            {
                dbQuery = dbQuery.Where(p => p.Width >= query.MinWidth);
            }

            var dbPagedResults = await dbQuery
                .OrderByDescending(p => p.CreateDate)
                .ToPagedResultAsync(query);

            var mappedResults = dbPagedResults
                .Items
                .Select(_imageAssetSummaryMapper.Map)
                .ToList();

            return dbPagedResults.ChangeType(mappedResults);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchImageAssetSummariesQuery query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
