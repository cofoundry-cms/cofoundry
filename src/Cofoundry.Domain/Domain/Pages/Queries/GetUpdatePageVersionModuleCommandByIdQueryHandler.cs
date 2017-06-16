using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetUpdatePageVersionModuleCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdatePageVersionModuleCommand>, UpdatePageVersionModuleCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdatePageVersionModuleCommand>, UpdatePageVersionModuleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;

        public GetUpdatePageVersionModuleCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper
            )
        {
            _dbContext = dbContext;
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
        }

        #endregion

        #region execution

        public async Task<UpdatePageVersionModuleCommand> ExecuteAsync(GetByIdQuery<UpdatePageVersionModuleCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => m.PageVersionModuleId == query.Id)
                .Select(m => new
                {
                    PageModule = m,
                    ModuleTypeFileName = m.PageModuleType.FileName
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = Map(dbResult.PageModule, dbResult.ModuleTypeFileName);
            return result;
        }

        #endregion

        #region private helpers

        private UpdatePageVersionModuleCommand Map(PageVersionModule pageVersionModule, string moduleTypeFileName)
        {
            var result = Mapper.Map<UpdatePageVersionModuleCommand>(pageVersionModule);
            result.DataModel = _pageVersionModuleModelMapper.MapDataModel(moduleTypeFileName, pageVersionModule);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdatePageVersionModuleCommand> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
