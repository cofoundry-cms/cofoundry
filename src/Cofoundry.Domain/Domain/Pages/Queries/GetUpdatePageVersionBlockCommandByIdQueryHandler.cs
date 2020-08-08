using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageVersionBlockCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageVersionBlockCommand>, UpdatePageVersionBlockCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageVersionBlockCommand>, UpdatePageVersionBlockCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;

        public GetUpdatePageVersionBlockCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper
            )
        {
            _dbContext = dbContext;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        }

        #endregion

        #region execution

        public async Task<UpdatePageVersionBlockCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageVersionBlockCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .Where(b => b.PageVersionBlockId == query.Id)
                .Select(b => new
                {
                    PageBlock = b,
                    BlockTypeFileName = b.PageBlockType.FileName
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = Map(dbResult.PageBlock, dbResult.BlockTypeFileName);
            return result;
        }

        private UpdatePageVersionBlockCommand Map(PageVersionBlock pageVersionBlock, string blockTypeFileName)
        {
            var result = new UpdatePageVersionBlockCommand()
            {
                PageBlockTypeId = pageVersionBlock.PageBlockTypeId,
                PageBlockTypeTemplateId = pageVersionBlock.PageBlockTypeTemplateId,
                PageVersionBlockId = pageVersionBlock.PageVersionBlockId
            };

            result.DataModel = _pageVersionBlockModelMapper.MapDataModel(blockTypeFileName, pageVersionBlock);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageVersionBlockCommand> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
