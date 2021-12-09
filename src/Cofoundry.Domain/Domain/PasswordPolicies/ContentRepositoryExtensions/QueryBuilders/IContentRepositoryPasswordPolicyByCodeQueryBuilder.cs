namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving password policies for a specified user area.
    /// </summary>
    public interface IContentRepositoryPasswordPolicyByCodeQueryBuilder
    {
        /// <summary>
        /// The <see cref="PasswordPolicyDescription"/> projection is used to describe 
        /// a password policy including both a short <see cref="Description"/> 
        /// and a more detailed list of <see cref="Requirements"/>.
        /// </summary>
        IDomainRepositoryQueryContext<PasswordPolicyDescription> AsDescription();
    }
}
