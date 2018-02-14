using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetUpdateCustomEntityVersionPageBlockCommandByIdQueryHandler
        : IAsyncQueryHandler<GetUpdateCommandByIdQuery<UpdateCustomEntityVersionPageBlockCommand>, UpdateCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateCustomEntityVersionPageBlockCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<UpdateCustomEntityVersionPageBlockCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdateCustomEntityVersionPageBlockCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .Where(b => b.CustomEntityVersionPageBlockId == query.Id)
                .Select(b => new
                {
                    PageBlock = b,
                    PageBlockTypeFileName = b.PageBlockType.FileName,
                    CustomEntityDefinitionCode = b.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(dbResult.CustomEntityDefinitionCode);

            var result = Map(dbResult.PageBlock, dbResult.PageBlockTypeFileName);
            return result;
        }

        private UpdateCustomEntityVersionPageBlockCommand Map(CustomEntityVersionPageBlock dbPageBlock, string pageBlockTypeFileName)
        {
            var result = new UpdateCustomEntityVersionPageBlockCommand()
            {
                CustomEntityVersionPageBlockId = dbPageBlock.CustomEntityVersionPageBlockId,
                PageBlockTypeId = dbPageBlock.PageBlockTypeId,
                PageBlockTypeTemplateId = dbPageBlock.PageBlockTypeTemplateId
            };
                
            result.DataModel = _pageVersionBlockModelMapper.MapDataModel(pageBlockTypeFileName, dbPageBlock);

            return result;
        }

        #endregion

    }
}
