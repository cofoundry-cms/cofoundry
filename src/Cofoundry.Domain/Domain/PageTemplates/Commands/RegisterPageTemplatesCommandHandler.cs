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
    /// Updates the page templates registered in the database by scanning
    /// for template files via IPageTemplateViewFileLocator. This is typically
    /// run during the auto-update process when the application starts up.
    /// 
    /// Removed templates are only deleted from the system if they have no
    /// dependencies, otherwise they are marked as archived which allows for
    /// data migration and prevents unintended deletions of page content.
    /// </summary>
    public class RegisterPageTemplatesCommandHandler
        : IAsyncCommandHandler<RegisterPageTemplatesCommand>
        , IPermissionRestrictedCommandHandler<RegisterPageTemplatesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IPageCache _pageCache;
        private readonly IPageTemplateViewFileLocator _pageTemplateViewFileLocator;

        public RegisterPageTemplatesCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IPageCache pageCache,
            IPageTemplateViewFileLocator pageTemplateViewFileLocator
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _pageCache = pageCache;
            _pageTemplateViewFileLocator = pageTemplateViewFileLocator;
        }

        #endregion

        public async Task ExecuteAsync(RegisterPageTemplatesCommand command, IExecutionContext executionContext)
        {
            var dbPageTemplates = await _dbContext
                .PageTemplates
                .Include(t => t.PageTemplateSections)
                .ToDictionaryAsync(d => d.FileName);

            var fileTemplates = _pageTemplateViewFileLocator
                .GetPageTemplateFiles();

            DetectDuplicateTemplates(fileTemplates);

            // Mark removed templates as deleted
            await DeleteTemplates(executionContext, dbPageTemplates, fileTemplates);
            await UpdateTemplates(executionContext, dbPageTemplates, fileTemplates);

            // Save changes
            await _dbContext.SaveChangesAsync();
            _pageCache.Clear();
        }

        private async Task UpdateTemplates(
            IExecutionContext executionContext, 
            Dictionary<string, PageTemplate> dbPageTemplates, 
            IEnumerable<PageTemplateFile> fileTemplates
            )
        {
            foreach (var fileTemplate in fileTemplates)
            {
                var fileTemplateDetails = await _queryExecutor.ExecuteAsync(new GetPageTemplateFileInfoByPathQuery(fileTemplate.FullPath), executionContext);
                EntityNotFoundException.ThrowIfNull(fileTemplateDetails, fileTemplate.FullPath);
                var dbPageTemplate = dbPageTemplates.GetOrDefault(fileTemplate.FileName);

                // Run this first because it may commit changes
                await EnsureCustomEntityDefinitionExists(fileTemplateDetails, dbPageTemplate);

                dbPageTemplate = await UpdateTemplate(executionContext, dbPageTemplate, fileTemplate, fileTemplateDetails);

                // No need to update archived template sections
                if (dbPageTemplate.IsArchived) continue;

                // Update Sections
                UpdateSections(fileTemplate, fileTemplateDetails, dbPageTemplate, executionContext);
            }
        }

        private async Task DeleteTemplates(
            IExecutionContext executionContext, 
            Dictionary<string, PageTemplate> dbPageTemplates, 
            IEnumerable<PageTemplateFile> fileTemplates
            )
        {
            foreach (var removedDbTemplate in dbPageTemplates
                .Where(t => !fileTemplates.Any(ft => ft.FileName == t.Key) && !t.Value.IsArchived)
                .Select(t => t.Value))
            {
                if (await IsTemplateInUse(removedDbTemplate))
                {
                    // In use, so leave it as soft deleted/archived
                    removedDbTemplate.IsArchived = true;
                    removedDbTemplate.UpdateDate = executionContext.ExecutionDate;
                }
                else
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
                var moduleTypes = string.Join(", ", duplicateTemplateFiles.Select(f => f.FullPath));
                throw new PageTemplateRegistrationException(
                    $"Duplicate template '{ duplicateTemplateFiles.Key }' detected. Conflicting templates: { moduleTypes }");
            }
        }

        /// <summary>
        /// Checks that a custom entity definition exists if it is required by the tempate. This
        /// can cause a DbContext.SaveChanges to run.
        /// </summary>
        private Task EnsureCustomEntityDefinitionExists(
            PageTemplateFileInfo fileTemplateDetails,
            PageTemplate dbPageTemplate
            )
        {
            var definitionCode = fileTemplateDetails.CustomEntityDefinition?.CustomEntityDefinitionCode;

            // Only update/check the definition if it has changed to potentially save a query
            if (!string.IsNullOrEmpty(definitionCode) && (dbPageTemplate == null || definitionCode != dbPageTemplate.CustomEntityDefinitionCode))
            {
                var command = new EnsureCustomEntityDefinitionExistsCommand(fileTemplateDetails.CustomEntityDefinition.CustomEntityDefinitionCode);
                return _commandExecutor.ExecuteAsync(command);
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
                throw new ApplicationException(msg);
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

            dbPageTemplate.Name = fileTemplate.Name;
            dbPageTemplate.Description = fileTemplateDetails.Description;
            dbPageTemplate.FullPath = fileTemplate.FullPath;
            dbPageTemplate.UpdateDate = executionContext.ExecutionDate;

            return dbPageTemplate;
        }

        private void UpdateSections(
            PageTemplateFile fileTemplate, 
            PageTemplateFileInfo fileTemplateDetails, 
            PageTemplate dbPageTemplate,
            IExecutionContext executionContext
            )
        {
            // De-dup section names
            var duplicateSectionName = fileTemplateDetails
                .Sections
                .GroupBy(s => s.Name.ToLowerInvariant())
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateSectionName != null)
            {
                throw new PageTemplateRegistrationException($"Dulpicate template section '{ duplicateSectionName.First().Name }' in template { fileTemplate.FullPath }");
            }

            // Deletions
            var sectionsToDelete = dbPageTemplate
                .PageTemplateSections
                .Where(ts => !fileTemplateDetails.Sections.Any(fs => ts.Name.Equals(fs.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var sectionToDelete in sectionsToDelete)
            {
                _dbContext.PageTemplateSections.Remove(sectionToDelete);
            }

            // Updates/Additions
            foreach (var fileSection in fileTemplateDetails.Sections)
            {
                var existing = dbPageTemplate
                    .PageTemplateSections
                    .FirstOrDefault(s => s.Name.Equals(fileSection.Name, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    existing = new PageTemplateSection();
                    existing.PageTemplate = dbPageTemplate;
                    existing.CreateDate = executionContext.ExecutionDate;
                    _dbContext.PageTemplateSections.Add(existing);
                }

                // casing might have changed
                if (existing.Name != fileSection.Name)
                {
                    existing.Name = fileSection.Name;
                    existing.UpdateDate = executionContext.ExecutionDate;
                }

                // this will detach section data but there's no migrating that...
                if (existing.IsCustomEntitySection != fileSection.IsCustomEntitySection)
                {
                    existing.IsCustomEntitySection = fileSection.IsCustomEntitySection;
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
            return pageTemplate.CustomEntityDefinitionCode != fileInfo.CustomEntityDefinition.CustomEntityDefinitionCode;
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
