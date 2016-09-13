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
    public class UpdateCustomEntityUrlCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        [Required]
        [StringLength(200)]
        [Slug]
        public virtual string UrlSlug { get; set; }

        [PositiveInteger]
        public int? LocaleId { get; set; }
    }
}
