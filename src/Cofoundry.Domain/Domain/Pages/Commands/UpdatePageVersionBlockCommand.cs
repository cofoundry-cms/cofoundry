using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdatePageVersionBlockCommand : ICommand, ILoggableCommand, IPageVersionBlockDataModelCommand
    {
        [Required]
        [PositiveInteger]
        public int PageVersionBlockId { get; set; }

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
