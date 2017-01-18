using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<DocumentAssetRenderDetails>, DocumentAssetRenderDetails>
        , IAsyncQueryHandler<GetByIdQuery<DocumentAssetRenderDetails>, DocumentAssetRenderDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<DocumentAssetRenderDetails>, DocumentAssetRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetDocumentAssetRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public DocumentAssetRenderDetails Execute(GetByIdQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var result = Query(query)
                .SingleOrDefault();

            return result;
        }

        public async Task<DocumentAssetRenderDetails> ExecuteAsync(GetByIdQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var result = await Query(query)
                .SingleOrDefaultAsync();

            return result;
        }

        private IQueryable<DocumentAssetRenderDetails> Query(GetByIdQuery<DocumentAssetRenderDetails> query)
        {
            return _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterById(query.Id)
                .ProjectTo<DocumentAssetRenderDetails>();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<DocumentAssetRenderDetails> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
