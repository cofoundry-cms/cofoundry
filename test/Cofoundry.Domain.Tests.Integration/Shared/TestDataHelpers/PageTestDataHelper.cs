using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create pages in test fixtures.
    /// </summary>
    public class PageTestDataHelper
    {
        private readonly DbDependentFixture _dbDependentFixture;

        public PageTestDataHelper(DbDependentFixture dbDependentFixture)
        {
            _dbDependentFixture = dbDependentFixture;
        }

        /// <summary>
        /// Adds a page directory that is parented to the the specified
        /// <paramref name="parentDirectoryId"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The PageDirectoryId of the newly created directory.</returns>
        public async Task<int> AddAsync(
            string uniqueData,
            int parentDirectoryId,
            Action<AddPageCommand> configration = null
            )
        {
            var command = CreateAddCommand(uniqueData, parentDirectoryId);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();

            return await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(command);
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageCommand"/> using a generic page
        /// template.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        public AddPageCommand CreateAddCommand(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData),
                PageType = PageType.Generic,
                PageTemplateId = _dbDependentFixture.SeededEntities.TestPageTemplateId
            };

            return command;
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageCommand"/> using a custom entity
        /// details page template.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        public AddPageCommand CreateAddCommandWithCustomEntityDetailsPage(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                PageType = PageType.CustomEntityDetails,
                PageTemplateId = _dbDependentFixture.SeededEntities.TestCustomEntityPageTemplateId,
                CustomEntityRoutingRule = new IdAndUrlSlugCustomEntityRoutingRule().RouteFormat
            };

            return command;
        }
    }
}
