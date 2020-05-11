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
    public class UpdateImageAssetCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int ImageAssetId { get; set; }

        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [ValidateObject]
        public IUploadedFile File { get; set; }

        [StringLength(130)]
        [Required]
        public string Title { get; set; }

        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

        public ICollection<string> Tags { get; set; }
    }
}
