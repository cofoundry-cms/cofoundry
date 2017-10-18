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
    public class GetDocumentAssetDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<DocumentAssetDetails>, DocumentAssetDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<DocumentAssetDetails>, DocumentAssetDetails>
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

        public async Task<DocumentAssetDetails> ExecuteAsync(GetByIdQuery<DocumentAssetDetails> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .DocumentAssets
                .AsNoTracking()
                .Include(a => a.DocumentAssetTags)
                .ThenInclude(a => a.Select(t => t.Tag))
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            var result = _documentAssetDetailsMapper.Map(dbResult);

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
