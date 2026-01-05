using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"> extension methods for rewrite rules.
/// </summary>
public static class IPermissionSetBuilderRewriteRuleExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for rewrite  rules.
        /// </summary>
        public IPermissionSetBuilder IncludeRewriteRule()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for rewrite rules.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeRewriteRule(Action<RewriteRulePermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for rewrite  rules.
        /// </summary>
        public IPermissionSetBuilder ExcludeRewriteRule()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for rewrite rules.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeRewriteRule(Action<RewriteRulePermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<RewriteRulePermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new RewriteRulePermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
