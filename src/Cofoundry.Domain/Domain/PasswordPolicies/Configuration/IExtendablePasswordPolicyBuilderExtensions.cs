using System;

namespace Cofoundry.Domain.Extendable
{
    public static class IExtendablePasswordPolicyBuilderExtensions
    {
        /// <summary>
        /// Casts the <paramref name="passwordPolicyBuilder"/> to 
        /// <see cref="IExtendablePasswordPolicyBuilder"/>, allowing access to extensibility
        /// points to developers making <see cref="IPasswordPolicyBuilder"/> extension methods.
        /// </summary>
        public static IExtendablePasswordPolicyBuilder AsExtendable(this IPasswordPolicyBuilder passwordPolicyBuilder)
        {
            if (passwordPolicyBuilder is IExtendablePasswordPolicyBuilder extendableBuiler)
            {
                return extendableBuiler;
            }

            throw new Exception($"An {nameof(IPasswordPolicyBuilder)} implementation should also implement {nameof(IExtendablePasswordPolicyBuilder)} to allow internal/plugin extendability.");
        }
    }
}