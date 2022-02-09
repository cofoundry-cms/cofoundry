using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain
{
    public static class IDomainRepositoryExtensions
    {
        /// <summary>
        /// Used to manage transactions for multiple domain commands.
        /// This abstraction is an enhanced version of 
        /// System.Transaction.TransactionScope and works in the same way.
        /// </summary>
        public static IDomainRepositoryTransactionManager Transactions(this IDomainRepository domainRepository)
        {
            var extendedContentRepositry = domainRepository.AsExtendableContentRepository();

            return extendedContentRepositry.ServiceProvider.GetRequiredService<IDomainRepositoryTransactionManager>();
        }

        /// <summary>
        /// Sets the execution context for any queries or commands chained off this 
        /// instance. Typically used to impersonate a user, elevate permissions or 
        /// maintain context in nested query or command execution.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context instance to use.
        /// </param>
        public static TRepository WithContext<TRepository>(this TRepository repository, IExecutionContext executionContext)
            where TRepository : IDomainRepository
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithExecutionContext(executor, executionContext));
        }

        public static TRepository WithContextx<TRepository>(this TRepository repository, IExecutionContext executionContext)
            where TRepository : IDomainRepository
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithExecutionContext(executor, executionContext));
        }

        /// <summary>
        /// Uses the specified <paramref name="userContext"/> to build a new <see cref="IExecutionContext"/>
        /// to run queries or commands under. Typically this is used to impersonate a user or 
        /// elevate permissions.
        /// </summary>
        /// <param name="userContext">
        /// The <see cref="IUserContext"/> to build into a new <see cref="IExecutionContext"/>.
        /// </param>
        public static TRepository WithContext<TRepository>(this TRepository repository, IUserContext userContext)
            where TRepository : IDomainRepository
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var executionContextFactory = extendedContentRepositry.ServiceProvider.GetRequiredService<IExecutionContextFactory>();

            return (TRepository) extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithUserContext(executor, executionContextFactory, userContext));
        }

        /// <summary>
        /// Execute queries or commands using the user context associated with the
        /// specified user area. This is useful when implementing multiple user areas
        /// whereby a client can be logged into multiple user accounts belonging to 
        /// different user areas. Use this to force execution to use the context
        /// of a specific user area rather than relying on the "ambient" or default.
        /// </summary>
        /// <typeparam name="TUserAreaDefinition">
        /// The user area to use when determining the signed in user to execute
        /// tasks with.
        /// </typeparam>
        public static IDomainRepository WithContext<TUserAreaDefinition>(this IDomainRepository repository)
            where TUserAreaDefinition : IUserAreaDefinition
        {
            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var userArea = extendedContentRepositry.ServiceProvider.GetRequiredService<TUserAreaDefinition>();
            var userContextService = extendedContentRepositry.ServiceProvider.GetRequiredService<IUserContextService>();
            var executionContextFactory = extendedContentRepositry.ServiceProvider.GetRequiredService<IExecutionContextFactory>();

            return extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithUserAreaContext(executor, userArea, userContextService, executionContextFactory));
        }

        /// <summary>
        /// Runs any queries or commands chained off this instance under
        /// the system user account which has no permission restrictions.
        /// This is useful when you need to perform an action that the currently
        /// signed in user does not have permission for, e.g. signing up a new
        /// user prior to sign.
        /// </summary>
        public static TRepository WithElevatedPermissions<TRepository>(this TRepository repository)
            where TRepository : IDomainRepository
        {
            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var executionContextFactory = extendedContentRepositry.ServiceProvider.GetRequiredService<IExecutionContextFactory>();

            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithElevatedPermissions(executor, executionContextFactory));
        }

        /// <summary>
        /// Allows you to chain mutator functions to run after execution of a query.
        /// </summary>
        /// <typeparam name="TResult">Query result type.</typeparam>
        /// <param name="query">Query to mutate.</param>
        /// <returns>A query context that allows chaining of mutator functions.</returns>
        public static IDomainRepositoryQueryContext<TResult> WithQuery<TResult>(this IDomainRepository domainRepository, IQuery<TResult> query)
        {
            var extendableContentRepository = domainRepository.AsExtendableContentRepository();

            return DomainRepositoryQueryContextFactory.Create(query, extendableContentRepository);
        }
    }
}