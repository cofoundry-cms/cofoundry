using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Sets the status of a custom entity to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    public class UnPublishCustomEntityCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Sets the status of a custom entity to un-published, but does not
        /// remove the publish date, which is preserved so that it
        /// can be used as a default when the user chooses to publish
        /// again.
        /// </summary>
        public UnPublishCustomEntityCommand()
        {
        }

        /// <summary>
        /// Sets the status of a custom entity to un-published, but does not
        /// remove the publish date, which is preserved so that it
        /// can be used as a default when the user chooses to publish
        /// again.
        /// </summary>
        /// <param name="customEntityId">The database id of the page to unpublish.</param>
        public UnPublishCustomEntityCommand(int customEntityId)
        {
            CustomEntityId = customEntityId;
        }

        /// <summary>
        /// The id of the custom entity to set un-published.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
