using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdRangeQueryHandler
        : IAsyncQueryHandler<GetByIdRangeQuery<DocumentAssetRenderDetails>, IDictionary<int, DocumentAssetRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetByIdRangeQuery<DocumentAssetRenderDetails>, IDictionary<int, DocumentAssetRenderDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;

        public GetDocumentAssetRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, DocumentAssetRenderDetails>> ExecuteAsync(GetByIdRangeQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            return await QueryDb(query).ToDictionaryAsync(d => d.DocumentAssetId);
        }

        private IQueryable<DocumentAssetRenderDetails> QueryDb(GetByIdRangeQuery<DocumentAssetRenderDetails> query)
        {
            return _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterByIds(query.Ids)
                .ProjectTo<DocumentAssetRenderDetails>();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdRangeQuery<DocumentAssetRenderDetails> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
