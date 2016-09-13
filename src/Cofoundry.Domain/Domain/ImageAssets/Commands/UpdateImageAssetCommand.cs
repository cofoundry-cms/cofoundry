using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdateImageAssetCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int ImageAssetId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ValidateObject]
        public IUploadedFile File { get; set; }

        [StringLength(120)]
        [Required]
        public string Title { get; set; }

        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

        public string[] Tags { get; set; }
    }
}
