using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain
{
    public static class IContentRepositoryExtensions
    {
        /// <summary>
        /// Sets the execution context for any queries or commands
        /// chained of this instance. Typically used to impersonate
        /// a user, elevate permissions or maintain context in nested
        /// query or command execution.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context instance to use.
        /// </param>
        public static IContentRepository WithExecutionContext(this IContentRepository contentRepository, IExecutionContext executionContext)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var extendedContentRepositry = contentRepository.AsExtendableContentRepository();
            var newRepository = extendedContentRepositry.ServiceProvider.GetRequiredService<IContentRepositoryWithCustomExecutionContext>();
            newRepository.SetExecutionContext(executionContext);

            return newRepository;
        }

        /// <summary>
        /// Runs any queries or commands chained off this instance under
        /// the system user account which has no permission restrictions.
        /// This is useful when you need to perform an action that the currently
        /// logged in user does not have permission for, e.g. signing up a new
        /// user prior to login.
        /// </summary>
        public static IContentRepository WithElevatedPermissions(this IContentRepository contentRepository)
        {
            var extendedApi = contentRepository.AsExtendableContentRepository();

            return extendedApi.ServiceProvider.GetRequiredService<IContentRepositoryWithElevatedPermissions>();
        }
    }
}
