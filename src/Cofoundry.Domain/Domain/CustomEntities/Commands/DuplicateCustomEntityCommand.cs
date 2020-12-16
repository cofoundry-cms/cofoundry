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
    /// Creates a new custom entity, copying from an existing custom entity.
    /// </summary>
    public class DuplicateCustomEntityCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the existing custom entity to copy from.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int CustomEntityToDuplicateId { get; set; }

        /// <summary>
        /// Optional id of the locale if used in a localized site.
        /// </summary>
        [PositiveInteger]
        public int? LocaleId { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the custom entity. If the 
        /// custom entity defintion has AutoUrlSlug enabled then this is used 
        /// to generate the UrlSlug.
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// A url slug is usually required, except if the custom entity defintion
        /// has AutoUrlSlug enabled, in which case it is auto-generated.
        /// </summary>
        [MaxLength(200)]
        [Slug]
        public string UrlSlug { get; set; }

        #region Ouput

        /// <summary>
        /// The database id of the newly created custom entity. This is set after the 
        /// command has been run.
        /// </summary>
        [OutputValue]
        public int OutputCustomEntityId { get; set; }

        #endregion
    }
}
