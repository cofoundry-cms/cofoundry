using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Searches document assets based on simple filter criteria and 
    /// returns a paged set of summary results. 
    /// </summary>
    public class SearchDocumentAssetSummariesQueryHandler 
        : IQueryHandler<SearchDocumentAssetSummariesQuery, PagedQueryResult<DocumentAssetSummary>>
        , IPermissionRestrictedQueryHandler<SearchDocumentAssetSummariesQuery, PagedQueryResult<DocumentAssetSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IDocumentAssetSummaryMapper _documentAssetSummaryMapper;

        public SearchDocumentAssetSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IDocumentAssetSummaryMapper documentAssetSummaryMapper
            )
        {
            _dbContext = dbContext;
            _documentAssetSummaryMapper = documentAssetSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<DocumentAssetSummary>> ExecuteAsync(SearchDocumentAssetSummariesQuery query, IExecutionContext executionContext)
        {
            IQueryable<DocumentAsset> dbQuery = _dbContext
                .DocumentAssets
                .AsNoTracking()
                .Include(a => a.Creator)
                .Include(a => a.Updater)
                .Include(a => a.DocumentAssetTags)
                .ThenInclude(a => a.Tag);

            // Filter by tags
            if (!string.IsNullOrEmpty(query.Tags))
            {
                var tags = TagParser.Split(query.Tags).ToList();

                foreach (string tag in tags)
                {
                    // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                    string localTag = tag;

                    dbQuery = dbQuery.Where(p => p.DocumentAssetTags
                        .Select(t => t.Tag.TagText)
                        .Contains(localTag)
                        );
                }
            }

            // Filter multple extensions
            if (!string.IsNullOrEmpty(query.FileExtensions))
            {
                var fileExtensions = query.FileExtensions.Split(new char[] { '.', ' ', ','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                dbQuery = dbQuery.Where(p => fileExtensions.Contains(p.FileExtension));
            }

            if (!string.IsNullOrWhiteSpace(query.FileExtension))
            {
                var formattedExtension = query.FileExtension.TrimStart('.');
                dbQuery = dbQuery.Where(p => p.FileExtension == formattedExtension);
            }

            var dbPagesResults = await dbQuery
                .OrderByDescending(p => p.CreateDate)
                .ToPagedResultAsync(query);

            var mappedResults = dbPagesResults
                .Items
                .Select(_documentAssetSummaryMapper.Map);

            return dbPagesResults.ChangeType(mappedResults);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchDocumentAssetSummariesQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
