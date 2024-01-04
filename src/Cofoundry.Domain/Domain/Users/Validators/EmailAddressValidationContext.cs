namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class EmailAddressValidationContext : IEmailAddressValidationContext
{
    public string? PropertyName { get; set; }

    public required string UserAreaCode { get; set; }

    public int? UserId { get; set; }

    public required EmailAddressFormattingResult? Email { get; set; }

    public required IExecutionContext ExecutionContext { get; set; }
}
