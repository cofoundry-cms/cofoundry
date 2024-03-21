using Microsoft.Data.SqlClient;
using System.Data;

namespace Cofoundry.Core.EntityFramework.Internal;

public class SqlParameterFactory : ISqlParameterFactory
{
    private static readonly Dictionary<Type, SqlDbType> _dbTypeMap = new()
    {
        { typeof(string),  SqlDbType.NVarChar },
        { typeof(int),  SqlDbType.Int },
        { typeof(decimal),  SqlDbType.Decimal },
        { typeof(long),  SqlDbType.BigInt },
        { typeof(bool),  SqlDbType.Bit },
        { typeof(DateTime),  SqlDbType.DateTime2 },
        { typeof(float),  SqlDbType.Float },
        { typeof(Guid),  SqlDbType.UniqueIdentifier }
    };

    public SqlParameter CreateOutputParameterByType(string name, Type t)
    {
        var outputParam = new SqlParameter(name, DBNull.Value)
        {
            Direction = ParameterDirection.Output
        };

        // If this is a non-null value nullable type, return the converted base type
        var type = Nullable.GetUnderlyingType(t) ?? t;

        if (_dbTypeMap.TryGetValue(type, out var value))
        {
            outputParam.SqlDbType = value;
        }

        switch (outputParam.SqlDbType)
        {
            case SqlDbType.NVarChar:
            case SqlDbType.VarChar:
                // Size required for variable size output types
                outputParam.Size = -1;
                break;
        }

        return outputParam;
    }
}
