namespace Cofoundry.Domain;

/// <summary>
/// The result of a <see cref="ValidateAuthorizedTaskTokenQuery"/>, which contains
/// details on any error found as well as additional data to help with executing the
/// task.
/// </summary>
public class AuthorizedTaskTokenValidationResult : ValidationQueryResult
{
    public AuthorizedTaskTokenValidationResult() : base() { }

    public AuthorizedTaskTokenValidationResult(ValidationError error) : base(error) { }

    /// <summary>
    /// If valid, then this will contain additional data to help with executing the
    /// task. If invalid then this will be <see langword="null"/>.
    /// </summary>
    public AuthorizedTaskTokenValidationResultData Data { get; set; }
}
