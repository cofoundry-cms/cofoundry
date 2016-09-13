using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class DeleteImageAssetCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int ImageAssetId { get; set; }
    }
}
