using Microsoft.Data.SqlClient;

namespace Cofoundry.Domain.Data;

/// <summary>
/// <para>
/// Indicates that a known error occurred during the execution of a 
/// stored procedure. An <see cref="ErrorNumber"/> reference should
/// be provided to indicate the error type, with known errors documented in 
/// <see cref="StoredProcedureErrorNumbers"/>
/// </para>
/// <para>
/// In SQLServer this usually maps to errors thrown using the throw 
/// keyword, but should be limited to known errors that can be caught
/// by the domain layer.
/// </para>
/// </summary>
public class StoredProcedureExecutionException : Exception
{
    const string DEFAULT_MESSAGE = "Error {1} occurred calling stored procedure {0}.";

    /// <summary>
    /// Creates a new instance using the default message.
    /// </summary>
    /// <param name="storedProcedureName">The name of the stored procedure that threw the error.</param>
    /// <param name="errorNumber">
    /// A numerical reference for the error type. Known errors should be documented in 
    /// <see cref="StoredProcedureErrorNumbers"/>. 
    /// </param>
    public StoredProcedureExecutionException(string storedProcedureName, int errorNumber)
        : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, errorNumber))
    {
        ErrorNumber = errorNumber;
    }

    /// <summary>
    /// Creates a new instance with an enhanced message with additional explaination.
    /// </summary>
    /// <param name="storedProcedureName">The name of the stored procedure that threw the error.</param>
    /// <param name="errorNumber">
    /// A numerical reference for the error type. Known errors should be documented in 
    /// <see cref="StoredProcedureErrorNumbers"/>. 
    /// </param>
    /// <param name="explaination">
    /// Additional information that can be added on to the end of the error message.
    /// </param>
    public StoredProcedureExecutionException(string storedProcedureName, int errorNumber, string explaination)
        : base(string.Format(DEFAULT_MESSAGE, storedProcedureName, errorNumber) + " " + explaination)
    {
        ErrorNumber = errorNumber;
    }

    /// <summary>
    /// Creates a new instance using the Procedure, Number and Message properties of 
    /// <paramref name="innerException"/>.
    /// </summary>
    /// <param name="innerException">The inner exception thrown by the database.</param>
    public StoredProcedureExecutionException(SqlException innerException)
        : base(string.Format(DEFAULT_MESSAGE, innerException.Procedure, innerException.Number) + " " + innerException.Message, innerException)
    {
        ErrorNumber = innerException.Number;
    }

    /// <summary>
    /// A reference number that can be used to understand the cause of the 
    /// error. Known errors should be documented in 
    /// <see cref="StoredProcedureErrorNumbers"/>. 
    /// </summary>
    public int ErrorNumber { get; set; }
}
