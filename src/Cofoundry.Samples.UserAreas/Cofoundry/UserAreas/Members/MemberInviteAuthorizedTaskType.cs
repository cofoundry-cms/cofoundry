using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class MemberInviteAuthorizedTaskType : IAuthorizedTaskTypeDefinition
    {
        /// <summary>
        /// Convention is to use a public constant to make it
        /// easier to reference the unique code.
        /// </summary>
        public const string Code = "MEMINV";

        /// <summary>
        /// A unique 6 character code that can be used to reference the type. 
        /// The code should contain only single-byte (non-unicode) characters
        /// and although case-insensitive, the convention is to use uppercase
        /// e.g. "COFACR" represents the Cofoundry account recovery task.
        /// </summary>
        public string AuthorizedTaskTypeCode => Code;

        /// <summary>
        /// A unique name that succintly describes the task. Max 20 characters.
        /// </summary>
        public string Name => "Member Invite";
    }
}
