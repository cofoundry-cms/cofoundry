using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<DocumentAssetRenderDetails>, DocumentAssetRenderDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<DocumentAssetRenderDetails>, DocumentAssetRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IDocumentAssetRenderDetailsMapper _documentAssetRenderDetailsMapper;

        public GetDocumentAssetRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IDocumentAssetRenderDetailsMapper documentAssetRenderDetailsMapper
            )
        {
            _dbContext = dbContext;
            _documentAssetRenderDetailsMapper = documentAssetRenderDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<DocumentAssetRenderDetails> ExecuteAsync(GetByIdQuery<DocumentAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            var mappedResult = _documentAssetRenderDetailsMapper.Map(dbResult);

            return mappedResult;
        }

        private IQueryable<DocumentAsset> Query(GetByIdQuery<DocumentAssetRenderDetails> query)
        {
            return _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterById(query.Id);
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
