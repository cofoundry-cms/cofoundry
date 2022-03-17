using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.EntityFramework;

public interface ISqlParameterFactory
{
    SqlParameter CreateOutputParameterByType(string name, Type t);
}
