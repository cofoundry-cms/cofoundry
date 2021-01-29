using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A repository to make it easier to get hold of in-code
    /// definitions to custom entities
    /// </summary>
    public interface ICustomEntityDefinitionRepository
    {
        ICustomEntityDefinition GetByCode(string code);
        IEnumerable<ICustomEntityDefinition> GetAll();
        ICustomEntityDefinition Get<TDefinition>() where TDefinition : ICustomEntityDefinition;
    }
}
