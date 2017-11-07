using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdateCustomEntityDraftVersionCommand : ICustomEntityDataModelCommand, ICommand, ILoggableCommand
    {
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        [MaxLength(200)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// If set to true, the version will be published after it has been updated.
        /// </summary>
        public bool Publish { get; set; }

        /// <summary>
        /// Set a value to alter the publish date, otherwise the existing or current date is used.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        [Required]
        [ValidateObject]
        public ICustomEntityDataModel Model { get; set; }
    }
}
