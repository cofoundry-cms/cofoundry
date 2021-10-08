using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Common mapping functionality for <see cref="RouteAccessRule"/> projections.
    /// </summary>
    public interface IRouteAccessRuleMapper
    {
        /// <summary>
        /// Maps an entity-specific <see cref="IEntityAccessRule"/> data to an <see cref="RouteAccessRule"/>.
        /// </summary>
        /// <param name="entityAccessRule">Database object to map. Cannot be <see langword="null"/>.</param>
        RouteAccessRule Map<TAccessRule>(TAccessRule entityAccessRule)
            where TAccessRule : IEntityAccessRule;
    }
}
