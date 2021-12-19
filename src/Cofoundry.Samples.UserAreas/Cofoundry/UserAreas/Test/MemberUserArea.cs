using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class MemberUserArea : IUserAreaDefinition
    {
        public const string Code = "MEM";

        public string UserAreaCode => Code;

        public string Name => "Member";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => true;

        public string LoginPath => "/members/login";

        public bool IsDefaultAuthScheme => false;

        public void ConfigureOptions(UserAreaOptions options)
        {
            options.Password.MinUniqueCharacters = 6;
        }
    }
}
