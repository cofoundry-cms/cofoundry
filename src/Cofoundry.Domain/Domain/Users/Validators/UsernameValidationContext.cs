namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UsernameValidationContext : IUsernameValidationContext
{
    public string? PropertyName { get; set; }

    public required string UserAreaCode { get; set; }

    public int? UserId { get; set; }

    public UsernameFormattingResult? Username { get; set; }

    public required IExecutionContext ExecutionContext { get; set; }
}
