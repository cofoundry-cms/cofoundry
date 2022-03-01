using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Used when a Sql Stored Procedure executes without error, but does not
    /// return the expected result e.g. when an output parameter should have
    /// always have a value, but does not.
    /// </summary>
    public class UnexpectedStoredProcedureResultException : Exception
    {
        const string DEFAULT_MESSAGE = "An error occurred calling stored procedure {0}. {1}";

        public UnexpectedStoredProcedureResultException(string storedProcedureName)
            : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, "An unexpected result was returned."))
        {
        }

        public UnexpectedStoredProcedureResultException(string storedProcedureName, string message)
            : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, message))
        {
        }
    }
}
