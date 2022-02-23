using System;

namespace Cofoundry.Domain.Extendable
{
    public static class IExtendablePermissionSetBuilderExtensions
    {
        /// <summary>
        /// Casts the builder instance as <see cref="IExtendablePermissionSetBuilder"/> to
        /// provide access to hidden functionality intended for extending a builder with
        /// bespoke features. These are advanced feature intended to be used for extension only 
        /// e.g. internally by Cofoundry, in plugins or custom extensions
        /// </summary>
        public static IExtendablePermissionSetBuilder AsExtendableBuilder(this IPermissionSetBuilder builder)
        {
            if (builder is IExtendablePermissionSetBuilder extendableBuilder)
            {
                return extendableBuilder;
            }

            throw new Exception($"An {nameof(IPermissionSetBuilder)} implementation should also implement {nameof(IExtendablePermissionSetBuilder)} to allow internal/plugin extendibility.");
        }
    }
}