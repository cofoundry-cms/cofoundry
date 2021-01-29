using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A representation of the identity of a custom 
    /// entity. Typically used to minimally  identify a 
    /// reference to a custom entity.
    /// </summary>
    public class CustomEntityIdentity
    {
        public CustomEntityIdentity() { }

        public CustomEntityIdentity(
            string customEntityDefinitionCode,
            int customEnttiyId
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            CustomEntityId = customEnttiyId;
        }

        /// <summary>
        /// Unique 6 letter code representing the type of custom entity.
        /// </summary>
        [Required]
        [StringLength(6, MinimumLength=6)]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Database id of the custom entity record.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
