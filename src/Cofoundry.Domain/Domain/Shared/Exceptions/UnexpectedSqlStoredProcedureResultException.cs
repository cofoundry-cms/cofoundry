using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UnexpectedSqlStoredProcedureResultException : Exception
    {
        const string DEFAULT_MESSAGE = "An error occured calling stored procedure {0}. {1}";

        public UnexpectedSqlStoredProcedureResultException(string storedProcedureName)
            : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, "An unexpected result was returned."))
        {
        }

        public UnexpectedSqlStoredProcedureResultException(string storedProcedureName, string message)
            : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, message))
        {
        }
    }
}
