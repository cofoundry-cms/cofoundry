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
    /// A representation of the identity of a custom entity.
    /// </summary>
    public class CustomEntityIdentity
    {
        [Required]
        [StringLength(6, MinimumLength=6)]
        public string CustomEntityDefinitionCode { get; set; }

        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
