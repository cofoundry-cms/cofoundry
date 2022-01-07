using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class EmailAddressValidationContext : IEmailAddressValidationContext
    {
        public string PropertyName { get; set; }

        public string UserAreaCode { get; set; }

        public int? UserId { get; set; }

        public EmailAddressFormattingResult Email { get; set; }

        public IExecutionContext ExecutionContext { get; set; }
    }
}
