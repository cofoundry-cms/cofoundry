using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdatePageTemplateSectionCommandHandler 
        : IAsyncCommandHandler<UpdatePageTemplateSectionCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageTemplateSectionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageCache _pageCache;

        public UpdatePageTemplateSectionCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageCache pageCache
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageCache = pageCache;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdatePageTemplateSectionCommand command, IExecutionContext executionContext)
        {
            var section = await _dbContext
                .PageTemplateSections
                .SingleOrDefaultAsync(l => l.PageTemplateSectionId == command.PageTemplateSectionId);

            EntityNotFoundException.ThrowIfNull(section, command.PageTemplateSectionId);

            await ValidateIsSectionUnique(command, section.PageTemplateId);

            section.Name = command.Name;
            section.IsCustomEntitySection = command.IsCustomEntitySection;

            await _dbContext.SaveChangesAsync();

            _pageCache.Clear();
        }

        private async Task ValidateIsSectionUnique(UpdatePageTemplateSectionCommand command, int pageTemplateId)
        {
            var query = new IsPageTemplateSectionNameUniqueQuery();
            query.PageTemplateId = pageTemplateId;
            query.PageTemplateSectionId = command.PageTemplateSectionId;
            query.Name = command.Name;

            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("A section already exists with the name '{0}' for that template", command.Name);
                throw new UniqueConstraintViolationException(message, "Name", command.Name);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageTemplateSectionCommand command)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
