using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class MemberUserArea : IUserAreaDefinition
    {
        public const string Code = "Member";

        public string UserAreaCode => Code;

        public string Name => "Member";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => true;

        public string LoginPath => "/members/login";

        public bool IsDefaultAuthScheme => true;

        public void ConfigureOptions(UserAreaOptions options)
        {
            options.Password.MinUniqueCharacters = 6;
        }
    }
}
