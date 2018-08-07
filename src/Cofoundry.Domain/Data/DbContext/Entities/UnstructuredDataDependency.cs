using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Contains a record of a relation between one entitiy and another
    /// when it's defined in unstructured data. Also contains information on how deletions
    /// should cascade for the relationship.
    /// </summary>
    public class UnstructuredDataDependency
    {
        public string RootEntityDefinitionCode { get; set; }

        public int RootEntityId { get; set; }

        public virtual EntityDefinition RootEntityDefinition { get; set; }
        
        public string RelatedEntityDefinitionCode { get; set; }

        public int RelatedEntityId { get; set; }

        public virtual EntityDefinition RelatedEntityDefinition { get; set; }

        public int RelatedEntityCascadeActionId { get; set; }
    }
}
