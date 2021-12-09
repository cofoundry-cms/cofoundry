using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    public class NotSequentialNewPasswordValidator : INewPasswordValidator
    {
        private static string ERROR_CODE = NewPasswordValidationErrorCodes.AddNamespace("not-sequential");

        public string Criteria => $"Must not be a sequence of numbers or characters.";

        public ValidationError Validate(INewPasswordValidationContext context)
        {
            var lowerPassword = context
                .Password
                .ToLowerInvariant();

            if (IsCodeSequence(lowerPassword))
            {
                return new ValidationError()
                {
                    ErrorCode = ERROR_CODE,
                    Message = $"Password must not be a sequence of numbers or characters.",
                    Properties = new string[] { context.PropertyName }
                };
            }

            return null;
        }

        private bool IsCodeSequence(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var isSequence = true;
            var isReverseSequence = true;

            char lastChar = password[0];

            for (int i = 1; i < password.Length; i++)
            {
                if (lastChar != (password[i] + 1)) isReverseSequence = false;
                if (lastChar != (password[i] - 1)) isSequence = false;

                if (!isSequence && !isReverseSequence) return false;
                lastChar = password[i];
            }

            return isReverseSequence || isSequence;
        }
    }
}
