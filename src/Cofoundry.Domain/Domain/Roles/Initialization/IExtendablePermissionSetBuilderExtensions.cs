namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Extensions for extending <see cref="IPermissionSetBuilder"/>.
/// </summary>
public static class IExtendablePermissionSetBuilderExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Casts the builder instance as <see cref="IExtendablePermissionSetBuilder"/> to
        /// provide access to hidden functionality intended for extending a builder with
        /// bespoke features. These are advanced feature intended to be used for extension only 
        /// e.g. internally by Cofoundry, in plugins or custom extensions
        /// </summary>
        public IExtendablePermissionSetBuilder AsExtendableBuilder()
        {
            if (builder is IExtendablePermissionSetBuilder extendableBuilder)
            {
                return extendableBuilder;
            }

            throw new Exception($"An {nameof(IPermissionSetBuilder)} implementation should also implement {nameof(IExtendablePermissionSetBuilder)} to allow internal/plugin extendibility.");
        }
    }
}
