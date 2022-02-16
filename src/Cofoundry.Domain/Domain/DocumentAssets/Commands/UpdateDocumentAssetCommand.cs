using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the properties of an existing document asset. Updating
    /// the file is optional, but if you do then existing links to the
    /// asset file will redirect to the new asset file.
    /// </summary>
    public class UpdateDocumentAssetCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Database id of the document to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int DocumentAssetId { get; set; }

        /// <summary>
        /// Optional file source to use when updating the file
        /// for this document asset. If the file is updated then
        /// existing links to the asset file will redirect to the 
        /// new asset file. The <see cref="IFileSource"/> abstraction is
        /// used here to support multiple types of file source e.g. 
        /// FormFileSource, <see cref="EmbeddedResourceFileSource"/>.
        /// or <see cref="StreamFileSource"/>.
        /// </summary>
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [ValidateObject]
        public IFileSource File { get; set; }

        /// <summary>
        /// A short descriptive title of the document (130 characters).
        /// </summary>
        [StringLength(130)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// A longer description of the document in plain text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
        public ICollection<string> Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DocumentAssetCommandHelper.Validate(File);
        }
    }
}