namespace Cofoundry.Domain;

/// <summary>
/// Use this interface to create a custom <see cref="IEmailAddressUniquifier"/>
/// implementation for a specific user area, allowing you to customize the 
/// email address uniquification process without affecting other user areas.
/// The DI system will automatically pick up your implementation during DI
/// registration.
public interface IEmailAddressUniquifier<TUserArea> : IEmailAddressUniquifier
    where TUserArea : IUserAreaDefinition
{
}
