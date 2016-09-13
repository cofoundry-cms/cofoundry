using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddPageTemplateSectionCommandHandler 
        : IAsyncCommandHandler<AddPageTemplateSectionCommand>
        , IPermissionRestrictedCommandHandler<AddPageTemplateSectionCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageCache _pageCache;

        public AddPageTemplateSectionCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            EntityAuditHelper entityAuditHelper,
            IPageCache pageCache
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _entityAuditHelper = entityAuditHelper;
            _pageCache = pageCache;
        }

        public async Task ExecuteAsync(AddPageTemplateSectionCommand command, IExecutionContext executionContext)
        {
            await ValidateIsSectionUnique(command);

            var template = await _dbContext
                .PageTemplates
                .SingleOrDefaultAsync(l => l.PageTemplateId == command.PageTemplateId);
            EntityNotFoundException.ThrowIfNull(template, command.PageTemplateId);

            var section = new PageTemplateSection();
            section.Name = command.Name;
            section.PageTemplate = template;
            section.IsCustomEntitySection = command.IsCustomEntitySection;
            _entityAuditHelper.SetCreated(section, executionContext);

            var modulesToAdd = new List<PageModuleType>();

            if (!command.PermitAllModuleTypes)
            {
                modulesToAdd = await _dbContext
                    .PageModuleTypes
                    .Where(t => command.PermittedModuleTypeIds.Contains(t.PageModuleTypeId))
                    .ToListAsync();
            }
            else
            {
                modulesToAdd = await _dbContext
                    .PageModuleTypes
                    .Where(t => !t.IsCustom)
                    .ToListAsync();
            }

            if (!modulesToAdd.Any())
            {
                EntityNotFoundException.ThrowIfNull(modulesToAdd, command.PermitAllModuleTypes);
            }

            foreach (var moduleType in modulesToAdd)
            {
                section.PageModuleTypes.Add(moduleType);
            }

            _dbContext.PageTemplateSections.Add(section);

            await _dbContext.SaveChangesAsync();
            _pageCache.Clear();

            // Set Ouput
            command.OutputPageTemplateSectionId = section.PageTemplateSectionId;
        }

        private async Task ValidateIsSectionUnique(AddPageTemplateSectionCommand command)
        {
            var query = new IsPageTemplateSectionNameUniqueQuery();
            query.PageTemplateId = command.PageTemplateId;
            query.Name = command.Name;

            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("A section already exists with the name '{0}' for that template", command.Name);
                throw new UniqueConstraintViolationException(message, "Name", command.Name);
            }
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageTemplateSectionCommand command)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
