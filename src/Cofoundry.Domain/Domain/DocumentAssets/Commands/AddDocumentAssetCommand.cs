using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class AddDocumentAssetCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        [Required]
        [IgnoreDataMember]
        [JsonIgnore]
        [ValidateObject]
        public IUploadedFile File { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<string> Tags { get; set; }

        #region Output

        [OutputValue]
        public int OutputDocumentAssetId { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DocumentAssetCommandHelper.Validate(File);
        }
    }
}
