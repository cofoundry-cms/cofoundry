using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    public interface ISqlParameterFactory
    {
        SqlParameter CreateOutputParameterByType(string name, Type t);
    }
}
