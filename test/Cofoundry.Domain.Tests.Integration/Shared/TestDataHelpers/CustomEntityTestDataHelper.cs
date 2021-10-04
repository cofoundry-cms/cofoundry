using Cofoundry.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create custom entities in test fixtures.
    /// </summary>
    public class CustomEntityTestDataHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomEntityTestDataHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Adds an unpublished custom entity for the 
        /// <see cref="TestCustomEntityDefinition"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The CustomEntityId of the newly created custom entity.</returns>
        public async Task<int> AddAsync(
            string uniqueData,
            Action<AddCustomEntityCommand> configration = null
            )
        {
            var command = CreateAddCommand(uniqueData);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            return await contentRepository
                .CustomEntities()
                .AddAsync(command);
        }

        /// <summary>
        /// Creates a valid <see cref="AddCustomEntityCommand"/> for the 
        /// <see cref="TestCustomEntityDefinition"/> without the option to publish.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        public AddCustomEntityCommand CreateAddCommand(string uniqueData)
        {
            var command = new AddCustomEntityCommand()
            {
                Title = uniqueData,
                CustomEntityDefinitionCode = TestCustomEntityDefinition.DefinitionCode,
                Model = new TestCustomEntityDataModel(),
                UrlSlug = SlugFormatter.ToSlug(uniqueData)
            };

            return command;
        }
    }
}
