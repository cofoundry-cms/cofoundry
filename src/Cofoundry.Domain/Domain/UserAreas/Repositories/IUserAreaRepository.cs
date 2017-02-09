using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IUserAreaRepository
    {
        IUserAreaDefinition GetByCode(string code);
        IEnumerable<IUserAreaDefinition> GetAll();
    }
}
