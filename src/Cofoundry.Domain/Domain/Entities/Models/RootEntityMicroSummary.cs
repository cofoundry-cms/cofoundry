using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Very basic information about a root entity. Used mainly
    /// in queries that work out dependency graphs for related entities.
    /// </summary>
    public class RootEntityMicroSummary
    {
        public int RootEntityId { get; set; }

        public string RootEntityTitle { get; set; }

        public string EntityDefinitionCode { get; set; }

        public string EntityDefinitionName { get; set; }

        /// <summary>
        /// If this entity is versioned, does this relate to a previous version
        /// </summary>
        public bool IsPreviousVersion { get; set; }
    }
}
