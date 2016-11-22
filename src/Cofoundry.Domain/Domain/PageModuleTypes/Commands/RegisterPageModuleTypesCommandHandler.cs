using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the page module types registered in the database using the
    /// IPageModuleDataModel types registered in the DI injector. This is typically
    /// run during the auto-update process when the application starst up.
    /// </summary>
    public class RegisterPageModuleTypesCommandHandler
        : IAsyncCommandHandler<RegisterPageModuleTypesCommand>
        , IPermissionRestrictedCommandHandler<RegisterPageModuleTypesCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageCache _pageCache;
        private readonly IPageModuleTypeCache _moduleCache;
        private readonly IPageModuleDataModel[] _allPageModuleDataModels;
        
        public RegisterPageModuleTypesCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageCache pageCache,
            IPageModuleTypeCache moduleCache,
            IPageModuleDataModel[] allPageModuleDataModels
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageCache = pageCache;
            _allPageModuleDataModels = allPageModuleDataModels;
            _moduleCache = moduleCache;
        }

        public async Task ExecuteAsync(RegisterPageModuleTypesCommand command, IExecutionContext executionContext)
        {
            var dbPageModules = await _dbContext
                .PageModuleTypes
                .Include(t => t.PageModuleTemplates)
                .ToDictionaryAsync(d => d.FileName);

            DetectDuplicateModules();

            var moduleDataModels = _allPageModuleDataModels
                .ToDictionary(m => FormatModuleFileName(m));

            await DeleteModules(executionContext, dbPageModules, moduleDataModels);

            await UpdateModulesAsync(executionContext, dbPageModules, moduleDataModels);

            await _dbContext.SaveChangesAsync();
            _pageCache.Clear();
            _moduleCache.Clear();
        }

        private async Task UpdateModulesAsync(
            IExecutionContext executionContext, 
            Dictionary<string, PageModuleType> dbPageModules, 
            Dictionary<string, IPageModuleDataModel> moduleDataModels
            )
        {
            foreach (var model in moduleDataModels)
            {
                var fileName = model.Key;
                var existingModule = dbPageModules.GetOrDefault(fileName);
                bool isUpdated = false;

                var fileDetails = await _queryExecutor.ExecuteAsync(new GetPageModuleTypeFileDetailsByFileNameQuery(fileName), executionContext);
                var name = string.IsNullOrWhiteSpace(fileDetails.Name) ? TextFormatter.PascalCaseToSentence(fileName) : fileDetails.Name;
                if (existingModule == null)
                {
                    existingModule = new PageModuleType();
                    existingModule.FileName = fileName;
                    existingModule.CreateDate = executionContext.ExecutionDate;
                    isUpdated = true;
                }

                if (existingModule.IsArchived)
                {
                    isUpdated = true;
                    existingModule.IsArchived = false;
                }

                if (existingModule.Name != name)
                {
                    isUpdated = true;
                    existingModule.Name = name;
                }

                if (existingModule.Description != fileDetails.Description)
                {
                    isUpdated = true;
                    existingModule.Description = fileDetails.Description;
                }

                UpdateTemplates(executionContext, existingModule, fileDetails);

                if (isUpdated)
                {
                    existingModule.UpdateDate = executionContext.ExecutionDate;
                }
            }
        }

        private void UpdateTemplates(
            IExecutionContext executionContext, 
            PageModuleType existingModule, 
            PageModuleTypeFileDetails fileDetails
            )
        {
            var templatesToDelete = existingModule
                                .PageModuleTemplates
                                .Where(mt => !fileDetails.Templates.Any(t => t.FileName.Equals(mt.FileName, StringComparison.OrdinalIgnoreCase)))
                                .ToList();
            _dbContext.PageModuleTypeTemplates.RemoveRange(templatesToDelete);

            foreach (var fileTemplate in fileDetails.Templates)
            {
                var existingTemplate = existingModule
                    .PageModuleTemplates
                    .FirstOrDefault(t => t.FileName.Equals(fileTemplate.FileName, StringComparison.OrdinalIgnoreCase));

                if (existingTemplate == null)
                {
                    existingTemplate = new PageModuleTypeTemplate();
                    existingTemplate.CreateDate = executionContext.ExecutionDate;
                    existingModule.PageModuleTemplates.Add(existingTemplate);
                }

                existingTemplate.FileName = fileTemplate.FileName;
                existingTemplate.Name = fileTemplate.Name;
                existingTemplate.Description = fileTemplate.Description;
            }
        }

        private async Task DeleteModules(
            IExecutionContext executionContext,
            Dictionary<string, PageModuleType> dbPageModules,
            Dictionary<string, IPageModuleDataModel> moduleDataModels)
        {
            var modulesToDelete = dbPageModules
                .Where(m => !moduleDataModels.ContainsKey(m.Key) && !m.Value.IsArchived)
                .ToList();

            foreach (var moduleToDelete in modulesToDelete)
            {
                if (!await IsModuleInUse(moduleToDelete.Value.PageModuleTypeId))
                {
                    // Clean up if it's not being used
                    _dbContext.PageModuleTypes.Remove(moduleToDelete.Value);
                }
                else
                {
                    // Else archive to allow for later clean-up or migration
                    moduleToDelete.Value.IsArchived = true;
                    moduleToDelete.Value.UpdateDate = executionContext.ExecutionDate;
                }
            }
        }

        /// <remarks>
        /// We could potentially use namespacing here, but let's leave it out for
        /// now because it would throw up some issues - e.g. how would you reference it 
        /// by string, how would you ensure the template file was in a unique path?
        /// </remarks>
        private void DetectDuplicateModules()
        {
            var duplicateModuleDefinitions = _allPageModuleDataModels
                    .GroupBy(m => FormatModuleFileName(m))
                    .Where(m => m.Count() > 1)
                    .FirstOrDefault();

            if (!EnumerableHelper.IsNullOrEmpty(duplicateModuleDefinitions))
            {
                var moduleTypes = string.Join(", ", duplicateModuleDefinitions.Select(t => t.GetType().FullName));
                throw new PageModuleTypeRegistrationException(
                    $"Duplicate page module '{ duplicateModuleDefinitions.Key }' detected. Conflicting types: { moduleTypes }");
            }
        }

        private Task<bool> IsModuleInUse(int pageModuleId)
        {
            var isInUSe = _dbContext
                .PageModuleTypes
                .AsNoTracking()
                .Where(m => m.PageModuleTypeId == pageModuleId)
                .AnyAsync(m => m.PageVersionModules.Any() || m.CustomEntityVersionPageModules.Any());

            return isInUSe;
        }

        private string FormatModuleFileName<T>(T dataModel)
            where T : IPageModuleDataModel
        {
            const string DATA_MODEL_TEXT = "DataModel";
            var name = dataModel.GetType().Name;

            if (name.EndsWith(DATA_MODEL_TEXT, StringComparison.OrdinalIgnoreCase))
            {
                return name.Remove(name.Length - DATA_MODEL_TEXT.Length);
            }

            return name;
        }

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(RegisterPageModuleTypesCommand command)
        {
            // Permissions are tied to the page templating system

            yield return new PageTemplateCreatePermission();
            yield return new PageTemplateUpdatePermission();
            yield return new PageTemplateDeletePermission();
        }

        #endregion
    }
}
