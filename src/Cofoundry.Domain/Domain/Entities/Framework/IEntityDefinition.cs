using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IEntityDefinition
    {
        string EntityDefinitionCode { get; }

        /// <summary>
        /// Singlar name of the entity e.g. 'Page'
        /// </summary>
        string Name { get; }
    }
}
