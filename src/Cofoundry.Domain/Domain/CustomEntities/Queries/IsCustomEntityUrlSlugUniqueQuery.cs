using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if the UrlSlug property for a custom entity is invalid because it
    /// already exists. If the custom entity defition has ForceUrlSlugUniqueness 
    /// set to true then duplicates are not permitted, otherwise true will always
    /// be returned.
    /// </summary>
    public class IsCustomEntityUrlSlugUniqueQuery : IQuery<bool>
    {
        /// <summary>
        /// Optional database id of a custom entity to exclude from the check. 
        /// Used when checking an existing custom entity for uniqueness.
        /// </summary>
        public int? CustomEntityId { get; set; }

        /// <summary>
        /// The string identifier slug to check for uniqueness. 
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// Optional id of the locale if used in a localized site.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Unique 6 character code representing the type of custom entity
        /// being checked for uniqueness.
        /// </summary>
        [Required]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
