using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Removes a document asset from the system and
    /// queues any related files or caches to be removed
    /// as a separate process.
    /// </summary>
    public class DeleteDocumentAssetCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the document asset to remove.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int DocumentAssetId { get; set; }
    }
}
