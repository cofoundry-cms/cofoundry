using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        /// Sets the execution context for any queries or commands
        /// chained of this instance. Typically used to impersonate
        /// a user, elevate permissions or maintain context in nested
        /// query or command execution.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context instance to use.
        /// </param>
        public static IDomainRepository WithExecutionContext(this IDomainRepository domainRepository, IExecutionContext executionContext)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var extendedContentRepositry = domainRepository.AsExtendableContentRepository();
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
