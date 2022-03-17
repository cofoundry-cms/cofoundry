namespace Cofoundry.Web;

/// <summary>
/// A simple data container for returning the result of a command or query
/// from a rest api, structuring data and errors in a consistent response.
/// Wrapping data in this object prevents a potential JSON hijacking vulnerability.
/// </summary>
/// <typeparam name="T">Type of the data being returned</typeparam>
public class ApiResponseHelperResult<T> : ApiResponseHelperResult
{
    /// <summary>
    /// Any additional data to send back to the response.
    /// </summary>
    public T Data { get; set; }
}

/// <summary>
/// Represents the result of executing a command or function in a rest api,
/// structuring errors in a consistent response.
/// </summary>
public class ApiResponseHelperResult
{
    /// <summary>
    /// <see langword="true"/> if the request executed successfully; otherwise <see langword="false"/>.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Collection of any validation errors discovered when executing the request
    /// </summary>
    public ICollection<ValidationError> Errors { get; set; }
}
