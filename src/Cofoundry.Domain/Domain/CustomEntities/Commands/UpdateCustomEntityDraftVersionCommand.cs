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
    /// <summary>
    /// Updates the draft version of a custom entity. If a draft version
    /// does not exist then one is created first from the currently
    /// published version.
    /// </summary>
    public class UpdateCustomEntityDraftVersionCommand : ICustomEntityDataModelCommand, ICommand, ILoggableCommand
    {
        /// <summary>
        /// Unique 6 character code representing the type of custom entity.
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Database id of the custom enitity to update the draft version
        /// for. A custom entity can onl have one draft version. If a draft
        /// version does not exist then one is created from the currently
        /// published version.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// <para>
        /// The descriptive human-readable title of the custom entity.
        /// </para>
        /// <para>
        /// Changing the title of a custom entity does not change the url 
        /// if the custom entity defintion has AutoUrlSlug enabled. This is
        /// to prevent inadvertently breaking any urls that point to this
        /// resource. Instead use UpdateCustomEntityUrlCommand to change the 
        /// url.
        /// </para>
        /// </summary>
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

        /// <summary>
        /// The custom entity data model data to be validated and serialized 
        /// into the database.
        /// </summary>
        [Required]
        [ValidateObject]
        public ICustomEntityDataModel Model { get; set; }
    }
}
