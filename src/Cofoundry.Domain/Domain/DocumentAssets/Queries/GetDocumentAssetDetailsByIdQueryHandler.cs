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
    public class GetDocumentAssetDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<DocumentAssetDetails>, DocumentAssetDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<DocumentAssetDetails>, DocumentAssetDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetDocumentAssetDetailsByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<DocumentAssetDetails> ExecuteAsync(GetByIdQuery<DocumentAssetDetails> query, IExecutionContext executionContext)
        {
            var result = await _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterById(query.Id)
                .ProjectTo<DocumentAssetDetails>()
                .SingleOrDefaultAsync();

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<DocumentAssetDetails> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
