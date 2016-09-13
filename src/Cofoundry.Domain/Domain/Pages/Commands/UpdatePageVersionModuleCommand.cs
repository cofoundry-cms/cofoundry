using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdatePageVersionModuleCommand : ICommand, ILoggableCommand, IPageVersionModuleDataModelCommand
    {
        [Required]
        [PositiveInteger]
        public int PageVersionModuleId { get; set; }

        [PositiveInteger]
        public int? PageModuleTypeTemplateId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageModuleTypeId { get; set; }

        [Required]
        [ValidateObject]
        public IPageModuleDataModel DataModel { get; set; }
    }
}
