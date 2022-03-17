namespace Cofoundry.Domain;

/// <summary>
/// The authorized task type for user account verification requests.
/// </summary>
/// <inheritdoc/>
public class UserAccountVerificationAuthorizedTaskType : IAuthorizedTaskTypeDefinition
{
    public const string Code = "COFVER";

    public string AuthorizedTaskTypeCode { get; } = Code;

    public string Name { get; } = "Account Verification";
}
