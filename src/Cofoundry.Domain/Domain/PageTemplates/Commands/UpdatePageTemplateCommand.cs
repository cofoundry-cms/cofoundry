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
    public class UpdatePageTemplateCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageTemplateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomEntityModelType { get; set; }
    }
}
