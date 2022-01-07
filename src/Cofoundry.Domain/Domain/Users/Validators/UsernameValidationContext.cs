using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UsernameValidationContext : IUsernameValidationContext
    {
        public string PropertyName { get; set; }

        public string UserAreaCode { get; set; }

        public int? UserId { get; set; }

        public UsernameFormattingResult Username { get; set; }

        public IExecutionContext ExecutionContext { get; set; }
    }
}
