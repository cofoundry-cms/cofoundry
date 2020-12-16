using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.EntityFramework.Internal
{
    public class SqlParameterFactory : ISqlParameterFactory
    {
        private static readonly Dictionary<Type, SqlDbType> _dbTypeMap = new Dictionary<Type, SqlDbType>()
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
            var outputParam = new SqlParameter(name, DBNull.Value);
            outputParam.Direction = ParameterDirection.Output;

            // If this is a non-null value nullable type, return the converted base type
            var type = Nullable.GetUnderlyingType(t) ?? t;

            if (_dbTypeMap.ContainsKey(type))
            {
                outputParam.SqlDbType = _dbTypeMap[type];
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
}
