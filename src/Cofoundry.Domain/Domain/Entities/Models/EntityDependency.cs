using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class EntityDependency
    {
        public EntityDependency(string entityDefinitionCode, int entityId, bool isRequired)
        {
            EntityDefinitionCode = entityDefinitionCode;
            EntityId = entityId;

            RelatedEntityCascadeAction = isRequired ? RelatedEntityCascadeAction.None : RelatedEntityCascadeAction.CascadeProperty;
        }

        public string EntityDefinitionCode { get; set; }

        public int EntityId { get; set; }

        /// <summary>
        /// The action to take on the root entity if the related entity is deleted
        /// </summary>
        public RelatedEntityCascadeAction RelatedEntityCascadeAction { get; set; }
    }
}
