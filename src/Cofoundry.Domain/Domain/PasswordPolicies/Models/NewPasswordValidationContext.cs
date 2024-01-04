using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class NewPasswordValidationContext : INewPasswordValidationContext
{
    public string? PropertyName { get; set; }

    public string UserAreaCode { get; set; } = string.Empty;

    public int? UserId { get; set; }

    public string Password { get; set; } = string.Empty;

    public string? CurrentPassword { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public IExecutionContext ExecutionContext { get; set; } = CQS.Internal.ExecutionContext.Uninitialized;

    public static NewPasswordValidationContext MapFromUser(User user, IExecutionContext executionContext)
    {
        var context = new NewPasswordValidationContext()
        {
            Email = user.Email,
            UserAreaCode = string.IsNullOrEmpty(user.UserAreaCode) ? user.UserArea.UserAreaCode : user.UserAreaCode,
            UserId = user.UserId,
            Username = user.Username,
            ExecutionContext = executionContext
        };

        return context;
    }
}
