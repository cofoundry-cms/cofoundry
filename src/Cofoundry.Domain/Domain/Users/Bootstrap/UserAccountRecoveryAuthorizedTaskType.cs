namespace Cofoundry.Domain
{
    /// <summary>
    /// The authorized task type for user account recovery requests.
    /// </summary>
    /// <inheritdoc/>
    public class UserAccountRecoveryAuthorizedTaskType : IAuthorizedTaskTypeDefinition
    {
        public const string Code = "COFREC";

        public string AuthorizedTaskTypeCode { get; } = Code;

        public string Name { get; } = "Account Recovery";
    }
}
