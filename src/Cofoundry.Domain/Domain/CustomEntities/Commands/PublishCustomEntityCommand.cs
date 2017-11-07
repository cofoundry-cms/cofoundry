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
    public class PublishCustomEntityCommand : ICommand, ILoggableCommand
    {
        public PublishCustomEntityCommand()
        {
        }

        public PublishCustomEntityCommand(int customEntityId, DateTime? publishDate = null)
        {
            CustomEntityId = customEntityId;
            PublishDate = publishDate;
        }

        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Set a value to alter the publish date, otherwise the existing or current date is used.
        /// </summary>
        public DateTime? PublishDate { get; set; }
    }
}
