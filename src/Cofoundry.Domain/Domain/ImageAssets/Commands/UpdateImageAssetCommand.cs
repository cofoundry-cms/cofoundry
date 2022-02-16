using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the properties of an existing image asset. Updating
    /// the file is optional, but if you do then existing links to the
    /// asset file will redirect to the new asset file. The <see cref="IFileSource"/> 
    /// abstraction is used here to support multiple types of file source e.g. 
    /// FormFileSource, <see cref="EmbeddedResourceFileSource"/>. or <see cref="StreamFileSource"/>.
    /// </summary>
    public class UpdateImageAssetCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database if of the image asset to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int ImageAssetId { get; set; }

        /// <summary>
        /// Optional file source to use when updating the file
        /// for this image asset. If the file is updated then
        /// existing links to the asset file will redirect to the 
        /// new asset file.
        /// </summary>
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [ValidateObject]
        public IFileSource File { get; set; }

        /// <summary>
        /// The title or alt text for an image. Recommended to be up 
        /// 125 characters to accomodate screen readers.
        /// </summary>
        [StringLength(130)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The focal point to use when using dynamic cropping
        /// by default. 
        /// </summary>
        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
        public ICollection<string> Tags { get; set; }
    }
}
