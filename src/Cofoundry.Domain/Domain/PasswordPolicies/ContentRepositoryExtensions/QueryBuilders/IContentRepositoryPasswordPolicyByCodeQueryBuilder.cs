namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving password policies for a specified user area.
/// </summary>
public interface IContentRepositoryPasswordPolicyByCodeQueryBuilder
{
    /// <summary>
    /// The <see cref="PasswordPolicyDescription"/> projection is used to describe 
    /// a password policy including both a short description 
    /// and a more detailed list of requirements.
    /// </summary>
    IDomainRepositoryQueryContext<PasswordPolicyDescription> AsDescription();
}
