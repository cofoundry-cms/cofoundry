namespace Cofoundry.Domain
{
    /// <summary>
    /// A three way option for the user account status filter. 
    /// Note that deleted accounts are never returned from the search
    /// and so are not included in the filter.
    /// </summary>
    public enum UserAccountStatusFilter
    {
        /// <summary>
        /// Does not filter.
        /// </summary>
        Any,

        /// <summary>
        /// Only users that are active.
        /// </summary>
        Active,

        /// <summary>
        /// Only users that are inactive
        /// </summary>
        Deactivated
    }
}