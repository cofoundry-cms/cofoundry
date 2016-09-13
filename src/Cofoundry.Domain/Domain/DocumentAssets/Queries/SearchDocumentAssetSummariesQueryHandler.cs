using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class SearchDocumentAssetSummariesQueryHandler 
        : IAsyncQueryHandler<SearchDocumentAssetSummariesQuery, PagedQueryResult<DocumentAssetSummary>>
        , IPermissionRestrictedQueryHandler<SearchDocumentAssetSummariesQuery, PagedQueryResult<DocumentAssetSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public SearchDocumentAssetSummariesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<DocumentAssetSummary>> ExecuteAsync(SearchDocumentAssetSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .DocumentAssets
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

            var results = await dbQuery
                .OrderByDescending(p => p.CreateDate)
                .ProjectTo<DocumentAssetSummary>()
                .ToPagedResultAsync(query);

            return results;
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
