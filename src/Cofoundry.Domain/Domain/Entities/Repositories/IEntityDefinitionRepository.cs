using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public interface IEntityDefinitionRepository
    {
        IEntityDefinition GetByCode(string code);

        IEnumerable<IEntityDefinition> GetAll();
    }
}
