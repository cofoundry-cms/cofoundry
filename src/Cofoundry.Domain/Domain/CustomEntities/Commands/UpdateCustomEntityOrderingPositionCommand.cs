using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Changes the order of a single custom entity. The custom entity 
    /// definition must implement IOrderableCustomEntityDefintion to be 
    /// able to set ordering.
    /// </summary>
    public class UpdateCustomEntityOrderingPositionCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the custom entity to set the order for.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Optional integer representing the ordering of the entity within
        /// the collection of all currently ordered entities. Typically you'd
        /// use a loaded list of all entities to determine this value based
        /// on the existing ordering of elements.
        /// <para>
        /// If set to null this indicates no ordering is specified, which
        /// is typically used when the custom entity is set to use 
        /// CustomEntityOrdering.Partial mode.
        /// </para>
        /// </summary>
        [PositiveInteger]
        public int? Position { get; set; }
    }
}
