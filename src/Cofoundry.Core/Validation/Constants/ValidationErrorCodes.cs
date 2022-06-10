namespace Cofoundry.Core.Validation.Internal;

/// <summary>
/// Used to namespace Cofoundry error codes to prevent conflicts
/// e.g. "cf-new-password-max-length-exceeded" or "cf-page-not-unique"
/// </summary>
public class ValidationErrorCodes
{
    const string NAMESPACE = "cf-";

    /// <summary>
    /// Namespaces a validation error code, scoping it to Cofoundry to prevent
    /// conflicts with external code. Codes are expected to be in slug format 
    /// e.g. "cf-new-password-max-length-exceeded" or "cf-page-not-unique".
    /// </summary>
    /// <param name="errorCode">
    /// The code that succintly describes the error which should be in slug
    /// format e.g. "max-length-exceeded".
    /// </param>
    /// <param name="category">
    /// Optional category or entity name to futher namespace the error e.g. "new-password"
    /// or "page".
    /// </param>
    /// <returns></returns>
    public static string AddNamespace(string errorCode, string category = null)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(errorCode);

        if (!string.IsNullOrWhiteSpace(category))
        {
            return NAMESPACE + category + "-" + errorCode;
        }

        return NAMESPACE + errorCode; ;
    }
}
