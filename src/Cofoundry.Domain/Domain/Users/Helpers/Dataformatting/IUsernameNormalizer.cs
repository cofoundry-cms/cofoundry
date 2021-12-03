using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this interface to create a custom <see cref="IUsernameNormalizer"/>
    /// implementation for a specific user area, allowing you to customize the 
    /// username normalization process without affecting other user areas.
    /// The DI system will automatically pick up your implementation during DI
    /// registration.
    public interface IUsernameNormalizer<TUserArea> : IUsernameNormalizer
        where TUserArea : IUserAreaDefinition
    {
    }
}
