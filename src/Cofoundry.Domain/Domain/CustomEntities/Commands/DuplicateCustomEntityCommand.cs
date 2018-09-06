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
    public class DuplicateCustomEntityCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityToDuplicateId { get; set; }

        [PositiveInteger]
        public int? LocaleId { get; set; }

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

        [OutputValue]
        public int OutputCustomEntityId { get; set; }

        #endregion
    }
}
