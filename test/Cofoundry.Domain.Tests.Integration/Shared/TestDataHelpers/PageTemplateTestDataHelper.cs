using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Integration.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create or reference page 
    /// directories in test fixtures.
    /// </summary>
    public class PageTemplateTestDataHelper
    {
        private readonly DbDependentFixture _dbDependentFixture;

        public PageTemplateTestDataHelper(DbDependentFixture dbDependentFixture)
        {
            _dbDependentFixture = dbDependentFixture;
        }

        /// <summary>
        /// Adds a unique page template that references the "ArchivableTemplate" file
        /// using the provided <paramref name="uniqueData"/> as the template name.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use as the template file name in place of the real file name.
        /// </param>
        /// <returns>The PageTemplateId of the newly created page template.</returns>
        public async Task<int> AddMockTemplateAsync(string uniqueData)
        {
            using var scope = _dbDependentFixture.CreateServiceScope();
            var pageTemplateViewFileLocator = scope.GetRequiredService<IPageTemplateViewFileLocator>() as TestPageTemplateViewFileLocator;
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            if (pageTemplateViewFileLocator == null)
            {
                throw new InvalidOperationException($"In testing, {nameof(IPageTemplateViewFileLocator)} should be an instance of {nameof(TestPageTemplateViewFileLocator)}");
            }

            pageTemplateViewFileLocator.AddMockTemplateFile(new PageTemplateFile()
            {
                FileName = uniqueData,
                VirtualPath = "/Shared/SeedData/PageTemplates/MockTemplate.cshtml"
            });

            await contentRepository
                .WithElevatedPermissions()
                .ExecuteCommandAsync(new RegisterPageTemplatesCommand());

            var templateId = await dbContext
                .PageTemplates
                .Where(t => t.FileName == uniqueData)
                .Select(t => t.PageTemplateId)
                .SingleAsync();

            return templateId;
        }

        /// <summary>
        /// Forces a template to be set as archived directly in the database, which
        /// is usually only done if the template file is deleted. Only use this for
        /// mock templates created for specific tests.
        /// </summary>
        public async Task ArchiveTemplateAsync(int pageTemplateId)
        {
            using var scope = _dbDependentFixture.CreateServiceScope();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var template = await dbContext
                .PageTemplates
                .FilterByPageTemplateId(pageTemplateId)
                .SingleAsync();

            EntityNotFoundException.ThrowIfNull(template, pageTemplateId);

            template.IsArchived = true;

            await dbContext.SaveChangesAsync();
        }
    }
}
