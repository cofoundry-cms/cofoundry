using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Cofoundry.Core.Validation;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class UpdatePageTemplateCommandHandler 
        : IAsyncCommandHandler<UpdatePageTemplateCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageTemplateCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageCache _pageCache;
        private readonly PageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;
        private readonly ICommandExecutor _commandExecutor;

        public UpdatePageTemplateCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageCache pageCache,
            PageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper,
            ICommandExecutor commandExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageCache = pageCache;
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
            _commandExecutor = commandExecutor;
        }

        public async Task ExecuteAsync(UpdatePageTemplateCommand command, IExecutionContext executionContext)
        {
            var template = _dbContext
                .PageTemplates
                .SingleOrDefault(l => l.PageTemplateId == command.PageTemplateId);
            EntityNotFoundException.ThrowIfNull(template, command.PageTemplateId);

            if (!command.Name.Equals(template.Name, StringComparison.OrdinalIgnoreCase))
            {
                await ValidateIsUnique(command.Name, template);
            }

            command.Description = command.Description ?? string.Empty;

            await HandleCustomEntityData(command, template);

            Mapper.Map(command, template);

            await _dbContext.SaveChangesAsync();

            _pageCache.Clear();
        }

        private async Task HandleCustomEntityData(UpdatePageTemplateCommand command, PageTemplate template)
        {
            if (command.CustomEntityModelType != template.CustomEntityModelType)
            {
                if (template.CustomEntityModelType == null)
                {
                    throw new PropertyValidationException("CustomEntityModelType cannot be null", "CustomEntityModelType");
                }
                else if (template.PageTypeId != (int)PageType.CustomEntityDetails)
                {
                    throw new PropertyValidationException("Page Type does not allow a CustomEntityModelType", "CustomEntityModelType");
                }
                var modelType = _pageTemplateCustomEntityTypeMapper.Map(command.CustomEntityModelType);
                if (modelType == null)
                {
                    throw new PropertyValidationException("Model type not found", "CustomEntityModelType");
                }
                var query = new GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery();
                query.DisplayModelType = modelType;
                var definition = _queryExecutor.Execute(query);
                EntityNotFoundException.ThrowIfNull(definition, query.DisplayModelType);

                await _commandExecutor.ExecuteAsync(new EnsureCustomEntityDefinitionExistsCommand(definition.CustomEntityDefinitionCode));

                template.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
                template.CustomEntityModelType = command.CustomEntityModelType;
            }
        }

        private async Task ValidateIsUnique(string newName, PageTemplate template)
        {
            var query = new IsPageTemplateNameUniqueQuery();
            query.PageTemplateId = template.PageTemplateId;
            query.Name = newName;

            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("A template already exists with the name '{0}'", newName);
                throw new UniqueConstraintViolationException(message, "Name", newName);
            }
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageTemplateCommand command)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
