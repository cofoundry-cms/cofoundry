using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class DeleteDocumentAssetCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int DocumentAssetId { get; set; }
    }
}
