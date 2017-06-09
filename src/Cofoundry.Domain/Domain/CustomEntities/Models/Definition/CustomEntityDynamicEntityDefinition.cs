using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IEntityDefinition for custom entities. These are dynamically generated from the custom entitity
    /// definition classes.
    /// </summary>
    public class CustomEntityDynamicEntityDefinition : IDependableEntityDefinition
    {
        public CustomEntityDynamicEntityDefinition()
        {
        }

        public CustomEntityDynamicEntityDefinition(ICustomEntityDefinition customEntityDefinition)
        {
            if (customEntityDefinition == null) throw new ArgumentNullException(nameof(customEntityDefinition));

            EntityDefinitionCode = customEntityDefinition.CustomEntityDefinitionCode;
            Name = customEntityDefinition.Name;
        }

        public string EntityDefinitionCode { get; private set; }

        public string Name { get; private set; }

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetCustomEntityEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
