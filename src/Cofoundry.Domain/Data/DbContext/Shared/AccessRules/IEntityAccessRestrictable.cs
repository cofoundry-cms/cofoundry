using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents an entity that can be restricted using access rules.
    /// </summary>
    /// <typeparam name="TAccessRule">
    /// The entity-specific <see cref="IEntityAccessRule"/> implementation 
    /// e.g. <see cref="PageAccessRule"/>.
    /// </typeparam>
    public interface IEntityAccessRestrictable<TAccessRule>
        where TAccessRule : IEntityAccessRule
    {
        /// <summary>
        /// Integer representing the action that should be taken when a user
        /// does not meet the criteria of the access rules directly associated
        /// with this entity. This is mapped to the
        /// <see cref="Cofoundry.Domain.AccessRuleViolationAction"/> enum.
        /// </summary>
        int AccessRuleViolationActionId { get; set; }

        /// <summary>
        /// Unique 3 character code representing the <see cref="UserArea"/> with
        /// a login page to redirect to when a user does not meet the criteria of 
        /// the access rules directly associated with this entity.
        /// </summary>
        string UserAreaCodeForLoginRedirect { get; set; }

        /// <summary>
        /// The <see cref="UserAreaForLoginRedirect"/> with a login page to redirect to when a user 
        /// does not meet the criteria of the access rules directly associated with 
        /// this entity.
        /// </summary>
        UserArea UserAreaForLoginRedirect { get; set; }

        /// <summary>
        /// <para>
        /// Access rules are used to restrict access to a website resource to users
        /// fulfilling certain criteria such as a specific user area or role.
        /// </para>
        /// <para>
        /// Note that access rules do not apply to users from the Cofoundry Admin user
        /// area. They aren't intended to be used to restrict editor access in the admin UI 
        /// but instead are used to restrict public access to website pages and routes.
        /// </para>
        /// </summary>
        ICollection<TAccessRule> AccessRules { get; set; }

        /// <summary>
        /// Returns the database primary key of the entity instance.
        /// </summary>
        int GetId();
    }
}
