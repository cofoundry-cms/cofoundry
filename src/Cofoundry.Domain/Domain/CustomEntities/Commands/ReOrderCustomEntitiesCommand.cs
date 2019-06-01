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
    /// Reorders a set of custom entities. The custom entity definition must implement 
    /// IOrderableCustomEntityDefintion to be able to set ordering.
    /// </summary>
    public class ReOrderCustomEntitiesCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Unique 6 character code representing the type of custom entity
        /// to re-order items for. All items must be of the same custom entity
        /// type.
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Collection of all custom entity ids to have thier order set, in the order 
        /// they should appear. Any entities not included in the collection are assumed 
        /// to not require ordering and will have thier ordering set to null (and return 
        /// in a natural ordering)
        /// </summary>
        public ICollection<int> OrderedCustomEntityIds { get; set; }

        /// <summary>
        /// Optional locale id if these custom entities are partitioned by locale
        /// </summary>
        [PositiveInteger]
        public int? LocaleId { get; set; }
    }
}
