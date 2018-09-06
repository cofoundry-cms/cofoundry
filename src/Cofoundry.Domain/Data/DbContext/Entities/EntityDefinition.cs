using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class EntityDefinition
    {
        public EntityDefinition()
        {
            Permissions = new List<Permission>();
        }

        public string EntityDefinitionCode { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
