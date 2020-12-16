using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the page templates registered in the database by scanning
    /// for template files via IPageTemplateViewFileLocator. This is typically
    /// run during the auto-update process when the application starts up.
    /// 
    /// Removed templates are only deleted from the system if they have no
    /// dependencies, otherwise they are marked as archived which allows for
    /// data migration and prevents unintended deletions of page content.
    /// </summary>
    public class RegisterPageTemplatesCommandHandler
        : ICommandHandler<RegisterPageTemplatesCommand>
        , IPermissionRestrictedCommandHandler<RegisterPageTemplatesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IPageCache _pageCache;
        private readonly IPageTemplateViewFileLocator _pageTemplateViewFileLocator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public RegisterPageTemplatesCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IPageCache pageCache,
            IPageTemplateViewFileLocator pageTemplateViewFileLocator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _pageCache = pageCache;
            _pageTemplateViewFileLocator = pageTemplateViewFileLocator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        public async Task ExecuteAsync(RegisterPageTemplatesCommand command, IExecutionContext executionContext)
        {
            var dbPageTemplates = (await _dbContext
                .PageTemplates
                .Include(t => t.PageTemplateRegions)
                .ToListAsync())
                .ToLookup(d => d.FileName);

            var fileTemplates = _pageTemplateViewFileLocator
                .GetPageTemplateFiles()
                .ToList();

            DetectDuplicateTemplates(fileTemplates);

            // Mark removed templates as deleted
            await DeleteTemplates(executionContext, dbPageTemplates, fileTemplates);
            await UpdateTemplates(executionContext, dbPageTemplates, fileTemplates);

            // Save changes
            await _dbContext.SaveChangesAsync();
            _transactionScopeFactory.QueueCompletionTask(_dbContext, _pageCache.Clear);
        }

        private async Task UpdateTemplates(
            IExecutionContext executionContext,
            ILookup<string, PageTemplate> dbPageTemplates,
            ICollection<PageTemplateFile> fileTemplates
            )
        {
            foreach (var fileTemplate in fileTemplates)
            {
                var fileTemplateDetails = await _queryExecutor.ExecuteAsync(new GetPageTemplateFileInfoByPathQuery(fileTemplate.VirtualPath), executionContext);
                EntityNotFoundException.ThrowIfNull(fileTemplateDetails, fileTemplate.VirtualPath);
                var dbPageTemplate = EnumerableHelper.Enumerate(dbPageTemplates[fileTemplate.FileName])
                    .OrderBy(t => t.IsArchived)
                    .ThenByDescending(t => t.UpdateDate)
                    .FirstOrDefault();

                // Run this first because it may commit changes
                await EnsureCustomEntityDefinitionExistsAsync(fileTemplateDetails, dbPageTemplate, executionContext);

                dbPageTemplate = await UpdateTemplate(executionContext, dbPageTemplate, fileTemplate, fileTemplateDetails);

                // No need to update archived template regions
                if (dbPageTemplate.IsArchived) continue;

                // Update Regions
                UpdateRegions(fileTemplate, fileTemplateDetails, dbPageTemplate, executionContext);
            }
        }

        private async Task DeleteTemplates(
            IExecutionContext executionContext,
            ILookup<string, PageTemplate> dbPageTemplates,
            ICollection<PageTemplateFile> fileTemplates
            )
        {
            foreach (var removedDbTemplate in dbPageTemplates
                .Where(t => !fileTemplates.Any(ft => ft.FileName == t.Key))
                .SelectMany(t => t))
            {
                var isInUse = await IsTemplateInUse(removedDbTemplate);

                if (isInUse & !removedDbTemplate.IsArchived)
                {
                    // In use, so leave it as soft deleted/archived
                    removedDbTemplate.IsArchived = true;
                    removedDbTemplate.UpdateDate = executionContext.ExecutionDate;
                }
                else if (!isInUse)
                {
                    _dbContext.PageTemplates.Remove(removedDbTemplate);
                }
            }
        }

        private static void DetectDuplicateTemplates(IEnumerable<PageTemplateFile> fileTemplates)
        {
            var duplicateTemplateFiles = fileTemplates
                .GroupBy(f => f.FileName)
                .Where(f => f.Count() > 1)
                .FirstOrDefault();

            if (!EnumerableHelper.IsNullOrEmpty(duplicateTemplateFiles))
            {
                var templateList = string.Join(", ", duplicateTemplateFiles.Select(f => f.VirtualPath));
                throw new PageTemplateRegistrationException(
                    $"Duplicate template '{ duplicateTemplateFiles.Key }' detected. Conflicting templates: { templateList }");
            }
        }

        /// <summary>
        /// Checks that a custom entity definition exists if it is required by the tempate. This
        /// can cause a DbContext.SaveChanges to run.
        /// </summary>
        private Task EnsureCustomEntityDefinitionExistsAsync(
            PageTemplateFileInfo fileTemplateDetails,
            PageTemplate dbPageTemplate,
            IExecutionContext executionContext
            )
        {
            var definitionCode = fileTemplateDetails.CustomEntityDefinition?.CustomEntityDefinitionCode;

            // Only update/check the definition if it has changed to potentially save a query
            if (!string.IsNullOrEmpty(definitionCode) && (dbPageTemplate == null || definitionCode != dbPageTemplate.CustomEntityDefinitionCode))
            {
                var command = new EnsureCustomEntityDefinitionExistsCommand(fileTemplateDetails.CustomEntityDefinition.CustomEntityDefinitionCode);
                return _commandExecutor.ExecuteAsync(command, executionContext);
            }

            return Task.CompletedTask;
        }

        private async Task<PageTemplate> UpdateTemplate(
            IExecutionContext executionContext,
            PageTemplate dbPageTemplate,
            PageTemplateFile fileTemplate,
            PageTemplateFileInfo fileTemplateDetails
            )
        {
            bool isNew = false;

            if (dbPageTemplate == null)
            {
                isNew = true;

                dbPageTemplate = new PageTemplate();
                dbPageTemplate.FileName = fileTemplate.FileName;
                dbPageTemplate.CreateDate = executionContext.ExecutionDate;

                _dbContext.PageTemplates.Add(dbPageTemplate);
            }

            if (!isNew
                && HasCustomEntityDefinitionChanged(dbPageTemplate, fileTemplateDetails)
                && await IsTemplateInUse(dbPageTemplate))
            {
                var msg = "Cannot change the custom entity type associated with a page template once it is in use";
                throw new Exception(msg);
            }

            if (fileTemplateDetails.CustomEntityDefinition == null)
            {
                dbPageTemplate.PageTypeId = (int)PageType.Generic;
                dbPageTemplate.CustomEntityDefinitionCode = null;
                dbPageTemplate.CustomEntityModelType = null;
            }
            else
            {
                dbPageTemplate.PageTypeId = (int)PageType.CustomEntityDetails;
                dbPageTemplate.CustomEntityModelType = fileTemplateDetails.CustomEntityModelType;
                dbPageTemplate.CustomEntityDefinitionCode = fileTemplateDetails.CustomEntityDefinition.CustomEntityDefinitionCode;
            }

            if (dbPageTemplate.IsArchived)
            {
                // Re-add template that was previously deleted
                dbPageTemplate.IsArchived = false;
            }

            dbPageTemplate.Name = TextFormatter.PascalCaseToSentence(fileTemplate.FileName);
            dbPageTemplate.Description = fileTemplateDetails.Description;
            dbPageTemplate.FullPath = fileTemplate.VirtualPath;
            dbPageTemplate.UpdateDate = executionContext.ExecutionDate;

            ValidateTemplateProperties(dbPageTemplate);

            return dbPageTemplate;
        }

        /// <summary>
        /// Some properties of the template are generated based on the class name or file path and these
        /// should be validated to ensure that they do not exceed the database column sizes.
        /// </summary>
        private static void ValidateTemplateProperties(PageTemplate dbPageTemplate)
        {
            if (string.IsNullOrWhiteSpace(dbPageTemplate.Name))
            {
                throw new PageTemplateRegistrationException($"Page template name cannot be null. FileName: {dbPageTemplate.FileName}");
            }

            if (dbPageTemplate.Name.Length > 100)
            {
                throw new PageTemplateRegistrationException($"Page template name exceeds the maximum length of 100 characters: {dbPageTemplate.Name}");
            }

            if (dbPageTemplate.FileName.Length > 100)
            {
                throw new PageTemplateRegistrationException($"Page template file name exceeds the maximum length of 100 characters: {dbPageTemplate.FileName}");
            }

            if (dbPageTemplate.FullPath.Length > 400)
            {
                throw new PageTemplateRegistrationException($"Page template path exceeds the maximum length of 400 characters: {dbPageTemplate.FullPath}");
            }

            if (dbPageTemplate.CustomEntityModelType?.Length > 400)
            {
                throw new PageTemplateRegistrationException($"The custom entity class name for page template {dbPageTemplate.FileName} exceeds the maximum length of 400 characters: {dbPageTemplate.CustomEntityModelType}");
            }
        }

        private void UpdateRegions(
            PageTemplateFile fileTemplate,
            PageTemplateFileInfo fileTemplateDetails,
            PageTemplate dbPageTemplate,
            IExecutionContext executionContext
            )
        {
            // De-dup region names
            var duplicateRegionName = fileTemplateDetails
                .Regions
                .GroupBy(s => s.Name.ToLowerInvariant())
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateRegionName != null)
            {
                throw new PageTemplateRegistrationException($"Dulpicate template region '{ duplicateRegionName.First().Name }' in template { fileTemplate.VirtualPath }");
            }

            // Deletions
            var regionsToDelete = dbPageTemplate
                .PageTemplateRegions
                .Where(ts => !fileTemplateDetails.Regions.Any(fs => ts.Name.Equals(fs.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var regionToDelete in regionsToDelete)
            {
                _dbContext.PageTemplateRegions.Remove(regionToDelete);
            }

            // Updates/Additions
            foreach (var fileRegion in fileTemplateDetails.Regions)
            {
                var existing = dbPageTemplate
                    .PageTemplateRegions
                    .FirstOrDefault(s => s.Name.Equals(fileRegion.Name, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    existing = new PageTemplateRegion();
                    existing.PageTemplate = dbPageTemplate;
                    existing.CreateDate = executionContext.ExecutionDate;
                    _dbContext.PageTemplateRegions.Add(existing);
                }

                // casing might have changed
                if (existing.Name != fileRegion.Name)
                {
                    existing.Name = fileRegion.Name;
                    existing.UpdateDate = executionContext.ExecutionDate;
                }

                // this will detach region data but there's no migrating that...
                if (existing.IsCustomEntityRegion != fileRegion.IsCustomEntityRegion)
                {
                    existing.IsCustomEntityRegion = fileRegion.IsCustomEntityRegion;
                    existing.UpdateDate = executionContext.ExecutionDate;
                }
            }
        }

        /// <remarks>
        /// Here we have to check to see if the definition has changed because 
        /// changing it would break any pages that are using that template. The
        /// best approach for the developer would be to create a new template under a 
        /// different name
        /// </remarks>
        private bool HasCustomEntityDefinitionChanged(PageTemplate pageTemplate, PageTemplateFileInfo fileInfo)
        {
            // If generic page
            if (pageTemplate.CustomEntityDefinitionCode == null)
            {
                return fileInfo.CustomEntityDefinition != null;
            }

            // If custom entity page: compare details.
            return pageTemplate.CustomEntityDefinitionCode != fileInfo.CustomEntityDefinition?.CustomEntityDefinitionCode;
        }

        private Task<bool> IsTemplateInUse(PageTemplate pageTemplate)
        {
            return _dbContext
                .PageVersions
                .AsNoTracking()
                .AnyAsync(v => v.PageTemplateId == pageTemplate.PageTemplateId);
        }

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(RegisterPageTemplatesCommand command)
        {
            yield return new PageTemplateCreatePermission();
            yield return new PageTemplateUpdatePermission();
            yield return new PageTemplateDeletePermission();
        }

        #endregion
    }
}