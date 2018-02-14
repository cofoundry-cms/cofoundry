using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class AddImageAssetCommand : ICommand, ILoggableCommand
    {
        [Required]
        [IgnoreDataMember]
        [JsonIgnore]
        [ValidateObject]
        public IUploadedFile File { get; set; }

        [StringLength(120)]
        [Required]
        public string Title { get; set; }

        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

        public ICollection<string> Tags { get; set; }

        #region Output

        [OutputValue]
        public int OutputImageAssetId { get; set; }

        #endregion
    }
}
