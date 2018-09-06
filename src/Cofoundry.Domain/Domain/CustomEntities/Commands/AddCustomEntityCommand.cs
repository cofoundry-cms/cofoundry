using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class AddCustomEntityCommand : ICommand, ILoggableCommand, ICustomEntityDataModelCommand
    {
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        [Required]
        [ValidateObject]
        public ICustomEntityDataModel Model { get; set; }

        [PositiveInteger]
        public int? LocaleId { get; set; }

        /// <summary>
        /// A url slug is usually required, except if the custom entity defintion
        /// has AutoUrlSlug enabled, in which case it is auto-generated.
        /// </summary>
        [MaxLength(200)]
        [Slug]  
        public string UrlSlug { get; set; }

        [MaxLength(200)]
        [Required]
        public string Title { get; set; }

        public bool Publish { get; set; }

        public DateTime? PublishDate { get; set; }

        #region Ouput

        [OutputValue]
        public int OutputCustomEntityId { get; set; }

        #endregion
    }
}
