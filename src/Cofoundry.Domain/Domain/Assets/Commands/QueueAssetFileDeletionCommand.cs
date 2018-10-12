using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds an asset file reference to a queue so that it
    /// can be deleted by a background process.
    /// </summary>
    public class QueueAssetFileDeletionCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// The 6 character definition code of the asset entity i.e. 
        /// the image or document asset entity definition code.
        /// </summary>
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// The filename of the asset to delete without the file 
        /// extension. Copied directly from the deleted asset record.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FileNameOnDisk { get; set; }

        /// <summary>
        /// The file extension of the asset to delete without the 
        /// leading dot. Copied directly from the deleted asset record.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string FileExtension { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityDefinitionCode != ImageAssetEntityDefinition.DefinitionCode
                && EntityDefinitionCode != DocumentAssetEntityDefinition.DefinitionCode)
            {
                yield return new ValidationResult("Entity definition code can only be either the image asset or document asset definition code.", new string[] { nameof(EntityDefinitionCode) });
            }
        }
    }
}
