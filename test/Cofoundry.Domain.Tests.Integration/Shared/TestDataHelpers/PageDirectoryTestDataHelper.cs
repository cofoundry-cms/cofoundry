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
    /// Used to make it easier to create or reference page 
    /// directories in test fixtures.
    /// </summary>
    public class PageDirectoryTestDataHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public PageDirectoryTestDataHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<int> GetRootDirectoryIdAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();

            return await dbContext
                .PageDirectories
                .Where(d => !d.ParentPageDirectoryId.HasValue)
                .Select(d => d.PageDirectoryId)
                .SingleAsync();
        }

        /// <summary>
        /// Adds a page directory that is parented to the root
        /// page directory.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The PageDirectoryId of the newly created directory.</returns>
        public async Task<int> AddAsync(
            string uniqueData,
            Action<AddPageDirectoryCommand> configration = null
            )
        {
            var command = await CreateAddCommandAsync(uniqueData);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();

            return await contentRepository
                .WithElevatedPermissions()
                .PageDirectories()
                .AddAsync(command);
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
        public Task<int> AddAsync(
            string uniqueData,
            int parentDirectoryId,
            Action<AddPageDirectoryCommand> configration = null
            )
        {
            return AddAsync(uniqueData, command =>
            {
                command.ParentPageDirectoryId = parentDirectoryId;

                if (configration != null)
                {
                    configration(command);
                }
            });
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageDirectoryCommand"/> that is
        /// parented to the root directory.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        public async Task<AddPageDirectoryCommand> CreateAddCommandAsync(string uniqueData)
        {
            var rootDirectoryId = await GetRootDirectoryIdAsync();
            return CreateAddCommand(uniqueData, rootDirectoryId);
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageDirectoryCommand"/> that is
        /// parented to the specified <paramref name="parentDirectoryId"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        public AddPageDirectoryCommand CreateAddCommand(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageDirectoryCommand()
            {
                Name = uniqueData,
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData)
            };

            return command;
        }

        /// <summary>
        /// Adds access rule with an action of <see cref="AccessRuleViolationAction.Error"/>
        /// </summary>
        /// <param name="pageDirectoryId">Id of the directory to add the rule to.</param>
        /// <param name="userAreaCode">
        /// Unique 3 character code representing the user area to restrict
        /// the page to. This cannot be the Cofoundry admin user area, as 
        /// access rules do not apply to admin panel users.
        /// </param>
        /// <param name="roleId">
        /// Optionally restrict access to a specific role within the selected 
        /// user area.
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        public async Task AddAccessRuleAsync(
            int pageDirectoryId,
            string userAreaCode,
            int? roleId = null,
            Action<UpdatePageDirectoryAccessRulesCommand> configration = null
            )
        {
            var command = new UpdatePageDirectoryAccessRulesCommand()
            {
                PageDirectoryId = pageDirectoryId,
                ViolationAction = AccessRuleViolationAction.Error
            };

            command.AccessRules.AddNew(userAreaCode, roleId);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(command);
        }
    }
}
