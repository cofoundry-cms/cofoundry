using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class NewPasswordValidationContext : INewPasswordValidationContext
    {
        public string PropertyName { get; set; }

        public string UserAreaCode { get; set; }

        public int? UserId { get; set; }

        public string Password { get; set; }

        public string CurrentPassword { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IExecutionContext ExecutionContext { get; set; }

        public static NewPasswordValidationContext MapFromUser(User user)
        {
            var context = new NewPasswordValidationContext()
            {
                Email = user.Email,
                UserAreaCode = user.UserAreaCode ?? user.UserArea?.UserAreaCode,
                UserId = user.UserId,
                Username = user.Username
            };

            return context;
        }
    }
}
