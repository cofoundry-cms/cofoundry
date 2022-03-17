namespace Cofoundry.Domain;

/// <summary>
/// Use this interface to create a custom <see cref="IEmailAddressNormalizer"/>
/// implementation for a specific user area, allowing you to customize the 
/// email address normalization process without affecting other user areas.
/// The DI system will automatically pick up your implementation during DI
/// registration.
public interface IEmailAddressNormalizer<TUserArea> : IEmailAddressNormalizer
    where TUserArea : IUserAreaDefinition
{
}
