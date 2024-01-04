namespace Cofoundry.Domain;

/// <summary>
/// The result of a <see cref="ValidateAuthorizedTaskTokenQuery"/>, which contains
/// details on any error found as well as additional data to help with executing the
/// task.
/// </summary>
public class AuthorizedTaskTokenValidationResult : ValidationQueryResult
{
    public AuthorizedTaskTokenValidationResult() : base() { }

    public AuthorizedTaskTokenValidationResult(ValidationError? error) : base(error) { }

    /// <summary>
    /// Indicates if the authorization attempt was successful. If
    /// <see langword="true"/> then the <see cref="Data"/> property 
    /// should have a value.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Data))]
    public override bool IsSuccess { get => base.IsSuccess; set => base.IsSuccess = value; }

    /// <summary>
    /// Indicates the reason if the authorization failed. If authorization succeded 
    /// then this will be <see langword="null"/>.
    /// </summary>
    public override ValidationError? Error { get => base.Error; set => base.Error = value; }

    /// <summary>
    /// If valid, then this will contain additional data to help with executing the
    /// task. If invalid then this will be <see langword="null"/>.
    /// </summary>
    public AuthorizedTaskTokenValidationResultData? Data { get; set; }
}
