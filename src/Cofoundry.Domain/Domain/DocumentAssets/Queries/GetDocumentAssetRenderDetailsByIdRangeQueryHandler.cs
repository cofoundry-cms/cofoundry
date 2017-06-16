using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdRangeQueryHandler
        : IQueryHandler<GetByIdRangeQuery<DocumentAssetRenderDetails>, IDictionary<int, DocumentAssetRenderDetails>>
        , IAsyncQueryHandler<GetByIdRangeQuery<DocumentAssetRenderDetails>, IDictionary<int, DocumentAssetRenderDetails>>
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

        public IDictionary<int, DocumentAssetRenderDetails> Execute(GetByIdRangeQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            return QueryDb(query).ToDictionary(d => d.DocumentAssetId);
        }

        public async Task<IDictionary<int, DocumentAssetRenderDetails>> ExecuteAsync(GetByIdRangeQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            return await QueryDb(query).ToDictionaryAsync(d => d.DocumentAssetId);
        }

        #endregion

        #region private methods
        
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
