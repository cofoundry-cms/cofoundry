namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Base interface for <see cref="IDefaultPasswordPolicyConfiguration"/> and
    /// <see cref="IPasswordPolicyConfiguration{TUserArea}"/> which shouldn't be 
    /// implemented directly. Having this interface helps reduce confusion between the
    /// two publically implementable interfaces.
    /// </summary>
    public interface IPasswordPolicyConfigurationBase
    {
        /// <summary>
        /// Configures the password policy using the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">
        /// An empty <see cref="IPasswordPolicyBuilder"/> that can be used to configure
        /// the password policy. Use <see cref="IPasswordPolicyBuilderExtensions.UseDefaults"/>
        /// to use the Cofoundry defaults as a baseline.
        /// </param>
        void Configure(IPasswordPolicyBuilder builder);
    }
}
