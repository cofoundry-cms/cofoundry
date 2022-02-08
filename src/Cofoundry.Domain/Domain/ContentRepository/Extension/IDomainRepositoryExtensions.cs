using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
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
        public static IDomainRepository WithContext(this IDomainRepository repository, IExecutionContext executionContext)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var newRepository = extendedContentRepositry.ServiceProvider.GetRequiredService<IContentRepositoryWithCustomExecutionContext>();
            newRepository.SetExecutionContext(executionContext);

            return newRepository;
        }

        /// <summary>
        /// Uses the specified <paramref name="userContext"/> to build a new <see cref="IExecutionContext"/>
        /// to run queries or commands under. Typically this is used to impersonate a user or 
        /// elevate permissions.
        /// </summary>
        /// <param name="userContext">
        /// The <see cref="IUserContext"/> to build into a new <see cref="IExecutionContext"/>.
        /// </param>
        public static IDomainRepository WithContext(this IDomainRepository repository, IUserContext userContext)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var executionContextFactory = extendedContentRepositry.ServiceProvider.GetRequiredService<IExecutionContextFactory>();
            var executionContext = executionContextFactory.Create(userContext);

            return repository.WithContext(executionContext);
        }

        /// <summary>
        /// Runs any queries or commands chained off this instance under
        /// the system user account which has no permission restrictions.
        /// This is useful when you need to perform an action that the currently
        /// signed in user does not have permission for, e.g. signing up a new
        /// user prior to sign.
        /// </summary>
        public static IDomainRepository WithElevatedPermissions(this IDomainRepository domainRepository)
        {
            var extendedApi = domainRepository.AsExtendableContentRepository();

            return extendedApi.ServiceProvider.GetRequiredService<IContentRepositoryWithElevatedPermissions>();
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