using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdatePageTemplateSectionCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageTemplateSectionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsCustomEntitySection { get; set; }
    }
}
