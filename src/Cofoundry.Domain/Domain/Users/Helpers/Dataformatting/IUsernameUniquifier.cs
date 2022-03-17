namespace Cofoundry.Domain;

/// <summary>
/// Use this interface to create a custom <see cref="IUsernameUniquifier"/>
/// implementation for a specific user area, allowing you to customize the 
/// username uniquification process without affecting other user areas.
/// The DI system will automatically pick up your implementation during DI
/// registration.
public interface IUsernameUniquifier<TUserArea> : IUsernameUniquifier
    where TUserArea : IUserAreaDefinition
{
}
