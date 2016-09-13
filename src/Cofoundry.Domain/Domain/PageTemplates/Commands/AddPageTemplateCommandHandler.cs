using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.Validation;
using System.IO;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddPageTemplateCommandHandler 
        : IAsyncCommandHandler<AddPageTemplateCommand>
        , IPermissionRestrictedCommandHandler<AddPageTemplateCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly PageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public AddPageTemplateCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            EntityAuditHelper entityAuditHelper,
            PageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper,
            ICommandExecutor commandExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _entityAuditHelper = entityAuditHelper;
            _commandExecutor = commandExecutor;
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        }

        public async Task ExecuteAsync(AddPageTemplateCommand command, IExecutionContext executionContext)
        {
            await ValidateIsUnique(command, executionContext);

            var template = new PageTemplate();
            template.Name = command.Name;
            template.FullPath = command.FullPath;
            template.Description = command.Description ?? string.Empty;
            template.FileName = Path.GetFileNameWithoutExtension(command.FullPath).TrimStart('_');

            template.PageTypeId = (int)command.PageType;

            await HandleCustomEntityData(command, template, executionContext);
            _entityAuditHelper.SetCreated(template, executionContext);
            _dbContext.PageTemplates.Add(template);
            
            await MapSections(command, executionContext, template);

            await _dbContext.SaveChangesAsync();

            // Set Ouput
            command.OutputPageTemplateId = template.PageTemplateId;
        }

        private async Task MapSections(AddPageTemplateCommand command, IExecutionContext executionContext, PageTemplate template)
        {
            if (command.Sections != null)
            {
                var allModules = await _dbContext
                                    .PageModuleTypes
                                    .ToListAsync();

                foreach (var addSectionCommand in command.Sections)
                {
                    var section = new PageTemplateSection();
                    section.Name = addSectionCommand.Name;
                    section.IsCustomEntitySection = addSectionCommand.IsCustomEntitySection;
                    _entityAuditHelper.SetCreated(section, executionContext);

                    var modulesToAdd = new List<PageModuleType>();

                    if (!addSectionCommand.PermitAllModuleTypes)
                    {
                        modulesToAdd = allModules
                            .Where(t => addSectionCommand.PermittedModuleTypeIds.Contains(t.PageModuleTypeId))
                            .ToList();
                    }
                    else
                    {
                        modulesToAdd = allModules
                            .Where(m => !m.IsCustom)
                            .ToList();
                    }

                    if (!modulesToAdd.Any())
                    {
                        EntityNotFoundException.ThrowIfNull(modulesToAdd, addSectionCommand.PermitAllModuleTypes);
                    }

                    foreach (var moduleType in modulesToAdd)
                    {
                        section.PageModuleTypes.Add(moduleType);
                    }

                    template.PageTemplateSections.Add(section);
                }
            }
        }

        private async Task HandleCustomEntityData(AddPageTemplateCommand command, PageTemplate template, IExecutionContext executionContext)
        {
            if (command.CustomEntityModelType != null)
            {
                if (template.PageTypeId != (int)PageType.CustomEntityDetails)
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
                var definition = _queryExecutor.Execute(query, executionContext);
                EntityNotFoundException.ThrowIfNull(definition, query.DisplayModelType);

                await _commandExecutor.ExecuteAsync(new EnsureCustomEntityDefinitionExistsCommand(definition.CustomEntityDefinitionCode), executionContext);

                template.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
                template.CustomEntityModelType = command.CustomEntityModelType;
            }
        }

        private async Task ValidateIsUnique(AddPageTemplateCommand command, IExecutionContext executionContext)
        {
            var query = new IsPageTemplateNameUniqueQuery();
            query.Name = command.Name.Trim();

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                var message = string.Format("A template already exists with the name '{0}'", command.Name);
                throw new UniqueConstraintViolationException(message, "Name", command.Name);
            }

            isUnique = !_dbContext
                .PageTemplates
                .Any(t => t.FullPath == command.FullPath);

            if (!isUnique)
            {
                throw new UniqueConstraintViolationException("A template already exists at this path", "FullPath", command.Name);
            }
        }
        
        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageTemplateCommand command)
        {
            yield return new PageTemplateCreatePermission();
        }

        #endregion
    }
}
