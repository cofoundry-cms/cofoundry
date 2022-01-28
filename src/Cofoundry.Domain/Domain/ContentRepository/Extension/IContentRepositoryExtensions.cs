using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public static class IContentRepositoryExtensions
    {
        /// <summary>
        /// Sets the execution context for any queries or commands chained off this 
        /// instance. Typically used to impersonate a user, elevate permissions or 
        /// maintain context in nested query or command execution.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context instance to use.
        /// </param>
        public static IContentRepository WithContext(this IContentRepository repository, IExecutionContext executionContext)
        {
            return (IContentRepository)IDomainRepositoryExtensions.WithContext(repository, executionContext);
        }

        /// <summary>
        /// Uses the specified <paramref name="userContext"/> to build a new <see cref="IExecutionContext"/>
        /// to run queries or commands under. Typically this is used to impersonate a user or 
        /// elevate permissions.
        /// </summary>
        /// <param name="userContext">
        /// The <see cref="IUserContext"/> to build into a new <see cref="IExecutionContext"/>.
        /// </param>
        public static IContentRepository WithContext(this IContentRepository repository, IUserContext userContext)
        {
            return (IContentRepository)IDomainRepositoryExtensions.WithContext(repository, userContext);
        }

        /// <summary>
        /// Runs any queries or commands chained off this instance under
        /// the system user account which has no permission restrictions.
        /// This is useful when you need to perform an action that the currently
        /// logged in user does not have permission for, e.g. signing up a new
        /// user prior to login.
        /// </summary>
        public static IContentRepository WithElevatedPermissions(this IContentRepository repository)
        {
            return (IContentRepository)IDomainRepositoryExtensions.WithElevatedPermissions(repository);
        }
    }
}