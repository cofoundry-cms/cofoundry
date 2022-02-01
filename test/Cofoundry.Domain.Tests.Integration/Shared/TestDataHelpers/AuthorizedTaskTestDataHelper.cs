using Cofoundry.Domain.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create roles in test fixtures.
    /// </summary>
    public class AuthorizedTaskTestDataHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserTestDataHelper _userTestDataHelper;

        public AuthorizedTaskTestDataHelper(
            IServiceProvider serviceProvider,
            SeededEntities seededEntities
            )
        {
            _serviceProvider = serviceProvider;
            _userTestDataHelper = new UserTestDataHelper(serviceProvider, seededEntities);
        }

        /// <summary>
        /// Creates a authorized task scoped to a newly created user using the
        /// <see cref="TestAuthorizedTaskType1"/> task type.
        /// </summary>
        /// <param name="uniqueData">Unique data to use when creating the user.</param>
        /// <param name="authorizedTaskTypeCode">The <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/> to group tasks by.</param>
        /// <param name="configuration">Optional configuration action to customize the command.</param>
        public async Task<AddAuthorizedTaskCommand> AddWithNewUserAsync(
            string uniqueData,
            DateTime? mockDate,
            Action<AddAuthorizedTaskCommand> configuration = null
            )
        {
            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();
            var userId = await _userTestDataHelper.AddAsync(uniqueData);

            return await AddAsync(userId, mockDate, configuration);
        }

        /// <summary>
        /// Creates a new authorized task using the <see cref="TestAuthorizedTaskType1"/>
        /// task type.
        /// </summary>
        /// <param name="userId">Id of the user to scope the task to.</param>
        /// <param name="authorizedTaskTypeCode">The <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/> to group tasks by.</param>
        /// <param name="configuration">Optional configuration action to customize the command.</param>
        public async Task<AddAuthorizedTaskCommand> AddAsync(
            int userId,
            DateTime? mockDate,
            Action<AddAuthorizedTaskCommand> configuration = null
            )
        {
            var command = CreateAddCommand(userId, configuration);

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();

            if (mockDate.HasValue)
            {
                MockServicesHelper.MockDateTime(scope.ServiceProvider, mockDate.Value);
            }

            await contentRepository
                .AuthorizedTasks()
                .AddAsync(command);

            return command;
        }

        /// <summary>
        /// Creates a valid <see cref="AddAuthorizedTaskCommand"/> using the <see cref="TestAuthorizedTaskType1"/>
        /// task type.
        /// </summary>
        /// <param name="userId">Id of the user to scope the task to.</param>
        /// <param name="authorizedTaskTypeCode">The <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/> to group tasks by.</param>
        /// <param name="configuration">Optional configuration action to customize the command.</param>
        public AddAuthorizedTaskCommand CreateAddCommand(
            int userId,
            Action<AddAuthorizedTaskCommand> configuration = null)
        {
            var command = new AddAuthorizedTaskCommand()
            {
                AuthorizedTaskTypeCode = TestAuthorizedTaskType1.Code,
                UserId = userId
            };

            if (configuration != null)
            {
                configuration(command);
            }

            return command;
        }
    }
}