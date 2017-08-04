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
    public class UpdateCustomEntityVersionPageBlockCommand : ICommand, ILoggableCommand, IPageVersionBlockDataModelCommand
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityVersionPageBlockId { get; set; }

        [PositiveInteger]
        public int? PageBlockTypeTemplateId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageBlockTypeId { get; set; }

        [Required]
        [ValidateObject]
        public IPageBlockTypeDataModel DataModel { get; set; }
    }
}
