using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithUserContext(executor, executionContextFactory, userContext));
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

        /// <summary>
        /// Patches a command to modify the current state and then executes it.
        /// </summary>
        /// <typeparam name="TCommand">Type of command to patch and execute.</typeparam>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with existing data.
        /// </param>
        public static async Task PatchCommandAsync<TCommand>(this IDomainRepository repository, Action<TCommand> commandPatcher)
            where TCommand : IPatchableCommand
        {
            var query = new GetPatchableCommandQuery<TCommand>();
            var command = await repository.ExecuteQueryAsync(query);
            commandPatcher(command);

            await repository.ExecuteCommandAsync(command);
        }

        /// <summary>
        /// Patches a command to modify the current state and then executes it.
        /// </summary>
        /// <typeparam name="TCommand">Type of command to patch and execute.</typeparam>
        /// <param name="id">
        /// The integer database identifier of the entity associated with
        /// patchable command.
        /// </param>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with existing data.
        /// </param>
        public static async Task PatchCommandAsync<TCommand>(this IDomainRepository repository, int id, Action<TCommand> commandPatcher)
            where TCommand : IPatchableByIdCommand
        {
            var query = new GetPatchableCommandByIdQuery<TCommand>(id);
            var command = await repository.ExecuteQueryAsync(query);
            commandPatcher(command);

            await repository.ExecuteCommandAsync(command);
        }

        /// <summary>
        /// Prevents execution completing before a random duration has elapsed by padding the
        /// execution time using <see cref="Task.Delay"/>. This can help mitigate against time-based 
        /// enumeration attacks by extending the <paramref name="minDurationInMilliseconds"/>  beyond 
        /// the expected bounds of the completion time. For example, this could be used to mitigate harvesting 
        /// of valid usernames from login or forgot password pages by measuring the response times.
        /// </summary>
        /// <param name="minDurationInMilliseconds">
        /// The minimum duration to extend the exection to. The execution will not complete quicker
        /// than this value.
        /// </param>
        /// <param name="maxDurationInMilliseconds">
        /// The maximum duration to extend the exection to.
        /// </param>
        public static TRepository WithRandomDuration<TRepository>(
            this TRepository repository,
            int minDurationInMilliseconds,
            int maxDurationInMilliseconds
            )
            where TRepository : IDomainRepository
        {
            return WithRandomDuration(repository, new RandomizedExecutionDuration()
            {
                Enabled = true,
                MinInMilliseconds = minDurationInMilliseconds,
                MaxInMilliseconds = maxDurationInMilliseconds
            });
        }

        /// <summary>
        /// Prevents execution completing before a random duration has elapsed by padding the
        /// execution time using <see cref="Task.Delay"/>. This can help mitigate against time-based 
        /// enumeration attacks by extending the exection duration beyond the expected bounds 
        /// of the query or command completion time. For example, this could be used to mitigate harvesting 
        /// of valid usernames from login or forgot password pages by measuring the response times.
        /// </summary>
        /// <param name="duration">
        /// The parameters to use in extending the duration.
        /// </param>
        public static TRepository WithRandomDuration<TRepository>(
            this TRepository repository,
            RandomizedExecutionDuration duration
            )
            where TRepository : IDomainRepository
        {
            var extendedContentRepositry = repository.AsExtendableContentRepository();
            var executionDurationRandomizerScopeManager = extendedContentRepositry.ServiceProvider.GetRequiredService<IExecutionDurationRandomizerScopeManager>();

            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithRandomizedDuration(executor, executionDurationRandomizerScopeManager, duration));
        }
    }
}