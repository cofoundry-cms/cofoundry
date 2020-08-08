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
    public class GetDocumentAssetDetailsByIdQueryHandler 
        : IQueryHandler<GetDocumentAssetDetailsByIdQuery, DocumentAssetDetails>
        , IPermissionRestrictedQueryHandler<GetDocumentAssetDetailsByIdQuery, DocumentAssetDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IDocumentAssetDetailsMapper _documentAssetDetailsMapper;

        public GetDocumentAssetDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IDocumentAssetDetailsMapper documentAssetDetailsMapper
            )
        {
            _dbContext = dbContext;
            _documentAssetDetailsMapper = documentAssetDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<DocumentAssetDetails> ExecuteAsync(GetDocumentAssetDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .DocumentAssets
                .AsNoTracking()
                .Include(a => a.Creator)
                .Include(a => a.Updater)
                .Include(a => a.DocumentAssetTags)
                .ThenInclude(a => a.Tag)
                .FilterById(query.DocumentAssetId)
                .SingleOrDefaultAsync();

            var result = _documentAssetDetailsMapper.Map(dbResult);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetDetailsByIdQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
