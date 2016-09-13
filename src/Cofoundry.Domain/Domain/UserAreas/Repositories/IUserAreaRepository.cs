using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IUserAreaRepository
    {
        IUserArea GetByCode(string code);
        IEnumerable<IUserArea> GetAll();
    }
}
