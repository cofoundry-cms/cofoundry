namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Extension methods for extending <see cref="IPasswordPolicyBuilder"/>.
/// </summary>
public static class IExtendablePasswordPolicyBuilderExtensions
{
    extension(IPasswordPolicyBuilder passwordPolicyBuilder)
    {
        /// <summary>
        /// Casts the policy builder to <see cref="IExtendablePasswordPolicyBuilder"/>, allowing access
        /// to extensibility points to developers making <see cref="IPasswordPolicyBuilder"/> extension methods.
        /// </summary>
        public IExtendablePasswordPolicyBuilder AsExtendable()
        {
            if (passwordPolicyBuilder is IExtendablePasswordPolicyBuilder extendableBuiler)
            {
                return extendableBuiler;
            }

            throw new Exception($"An {nameof(IPasswordPolicyBuilder)} implementation should also implement {nameof(IExtendablePasswordPolicyBuilder)} to allow internal/plugin extendability.");
        }
    }
}
