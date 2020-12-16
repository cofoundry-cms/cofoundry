using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetDocumentAssetRenderDetailsByIdQueryHandler 
        : IQueryHandler<GetDocumentAssetRenderDetailsByIdQuery, DocumentAssetRenderDetails>
        , IPermissionRestrictedQueryHandler<GetDocumentAssetRenderDetailsByIdQuery, DocumentAssetRenderDetails>
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

        public async Task<DocumentAssetRenderDetails> ExecuteAsync(GetDocumentAssetRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            var mappedResult = _documentAssetRenderDetailsMapper.Map(dbResult);

            return mappedResult;
        }

        private IQueryable<DocumentAsset> Query(GetDocumentAssetRenderDetailsByIdQuery query)
        {
            return _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterById(query.DocumentAssetId);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetRenderDetailsByIdQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
