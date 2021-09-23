namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Library of error numbers/codes that can be thrown in SQL stored
    /// procedures and functions.
    /// </summary>
    public class StoredProcedureErrorNumbers
    {
        public static class Page_AddDraft
        {
            /// <summary>
            /// Unable to locate a version to copy from when creating
            /// a new draft.
            /// </summary>
            public const int NoVersionFoundToCopyFrom = 51001;

            /// <summary>
            /// The page already has a draft version so one could
            /// not be created.
            /// </summary>
            public const int DraftAlreadyExists = 51002;
        }
    }
}
