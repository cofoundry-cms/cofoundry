using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdatePageTemplateSectionModuleTypesCommandHandler 
        : IAsyncCommandHandler<UpdatePageTemplateSectionModuleTypesCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageTemplateSectionModuleTypesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public UpdatePageTemplateSectionModuleTypesCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdatePageTemplateSectionModuleTypesCommand command, IExecutionContext executionContext)
        {
            var section = await Query(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(section, command.PageTemplateSectionId);

            if (!command.PermitAllModuleTypes)
            {
                var requiredModules = await _dbContext
                    .PageModuleTypes
                    .Where(t => command.PermittedModuleTypeIds.Contains(t.PageModuleTypeId))
                    .ToListAsync();

                if (!requiredModules.Any())
                {
                    EntityNotFoundException.ThrowIfNull(requiredModules, command.PermitAllModuleTypes);
                }

                // Remove deleted modules
                section.PageModuleTypes.RemoveAll(m => !command
                    .PermittedModuleTypeIds
                    .Contains(m.PageModuleTypeId));

                // Add new modules
                var newModules = requiredModules
                    .Where(m => !section
                                    .PageModuleTypes
                                    .Any(t => t.PageModuleTypeId == m.PageModuleTypeId));

                foreach (var moduleType in newModules)
                {
                    section.PageModuleTypes.Add(moduleType);
                }
            }
            else
            {
                // Remove deleted modules
                section.PageModuleTypes.RemoveAll(m => m.IsCustom);
                var currentlySelectedModuleIds = section.PageModuleTypes.Select(t => t.PageModuleTypeId);

                var modulestoAdd = await _dbContext
                    .PageModuleTypes
                    .Where(t => !t.IsCustom && !currentlySelectedModuleIds.Contains(t.PageModuleTypeId))
                    .ToListAsync();

                foreach (var moduleType in modulestoAdd)
                {
                    section.PageModuleTypes.Add(moduleType);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region private helpers

        private IQueryable<PageTemplateSection> Query(UpdatePageTemplateSectionModuleTypesCommand command)
        {
            var section = _dbContext
                .PageTemplateSections
                .Include(l => l.PageModuleTypes)
                .Where(l => l.PageTemplateSectionId == command.PageTemplateSectionId);

            return section;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageTemplateSectionModuleTypesCommand command)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
