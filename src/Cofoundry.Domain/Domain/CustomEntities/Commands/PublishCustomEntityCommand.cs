using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Publishes a custom entity. If the custom entity is already published and
    /// a date is specified then the publish date will be updated.
    /// </summary>
    public class PublishCustomEntityCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Publishes a custom entity. If the custom entity is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        public PublishCustomEntityCommand()
        {
        }

        /// <summary>
        /// Publishes a custom entity. If the custom entity is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        /// <param name="customEntityId">The database id of the custom entity to publish.</param>
        /// <param name="publishDate">
        /// Optional time that the custom entity should be published and made public.
        /// If this is left null then the publish date is set to the current 
        /// date and the custom entity is made immediately available.
        /// </param>
        public PublishCustomEntityCommand(int customEntityId, DateTime? publishDate = null)
        {
            CustomEntityId = customEntityId;
            PublishDate = publishDate;
        }

        /// <summary>
        /// The database id of the custom entity to publish.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Optional time that the custom entity should be published and made public.
        /// If this is left null then the publish date is set to the current 
        /// date and the custom entity is made immediately available.
        /// </summary>
        public DateTime? PublishDate { get; set; }
    }
}
