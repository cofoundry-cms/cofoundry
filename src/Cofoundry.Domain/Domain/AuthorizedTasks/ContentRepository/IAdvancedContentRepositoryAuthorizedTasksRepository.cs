using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for authorized tasks.
    /// </summary>
    public interface IAdvancedContentRepositoryAuthorizedTaskRepository : IContentRepositoryPart
    {
        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per user area.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <returns>
        /// An <see cref="AuthorizedTaskTokenValidationResult"/> which contains
        /// details on any error found as well as additional data to help with executing the
        /// task.
        /// </returns>
        IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> ValidateAsync(ValidateAuthorizedTaskTokenQuery query);

        /// <summary>
        /// Adds a new authorized task record, generating a new authorization token
        /// that can be used to re-validate the task at a later date.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>
        /// A url-safe token used to identify and authenticate a task before it is executed. Tokens
        /// are typically formatted into a url to authorize tasks such as password resets or email
        /// verification.
        /// </returns>
        Task<string> AddAsync(AddAuthorizedTaskCommand command);

        /// <summary>
        /// Marks an authorized task as complete. The command does not validate 
        /// the authorization task or token, which is expected to be done prior 
        /// to invoking this command. To validate an auhtorized task token use
        /// <see cref="ValidateAsync"/>.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task CompleteAsync(CompleteAuthorizedTaskCommand command);

        /// <summary>
        /// Invalidates all incomplete tasks associated with a specific user. Invalidations 
        /// can optionally be limited to a specific task type or range of task types by 
        /// specifying <see cref="InvalidateAuthorizedTaskBatchCommand.AuthorizedTaskTypeCodes"/>.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task InvalidateBatchAsync(InvalidateAuthorizedTaskBatchCommand command);
    }
}
