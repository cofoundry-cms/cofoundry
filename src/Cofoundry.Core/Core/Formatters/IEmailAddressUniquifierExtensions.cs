namespace Cofoundry.Core
{
    public static class IEmailAddressUniquifierExtensions
    {
        /// <summary>
        /// <para>
        /// Format an email address into a value that can be used for
        /// uniqueness comparisons. The default implementation simply lowercases
        /// the email, but this can be overriden to provide more strict uniqueness
        /// checks.
        /// </para>
        /// <para>
        /// As an example, if the user types their email as " L.Balfour+cofoundry@Example.com" then by
        /// default the email would normalize via <see cref="IEmailAddressNormalizer"/> 
        /// as "L.Balfour+cofoundry@example.com" and uniquify as "l.balfour+cofoundry@example.com",
        /// however if you wanted to exclude "plus addressing" from email uniqueness checks, you could 
        /// override the default <see cref="IEmailAddressUniquifier"/> implementation with your own
        /// custom implementation. 
        /// </para>
        /// </summary>
        /// <param name="emailAddress">
        /// The email address to uniquify. If the value is <see langword="null"/> 
        /// then <see langword="null"/> is returned.
        /// </param>
        public static string Uniquify(this IEmailAddressUniquifier uniquifier, string emailAddress)
        {
            return uniquifier
                .UniquifyAsParts(emailAddress)
                ?.ToEmailAddress();
        }

        /// <summary>
        /// Format an email address into a value that can be used for
        /// uniqueness comparisons. The default implementation simply lowercases
        /// the email, but this can be overriden to provide more strict uniqueness
        /// checks.
        /// </summary>
        /// <param name="emailAddressParts">
        /// The pre-normalized email address to uniquify. If the value is <see langword="null"/> 
        /// then <see langword="null"/> is returned.
        /// </param>
        public static string Uniquify(this IEmailAddressUniquifier uniquifier, NormalizedEmailAddress emailAddressParts)
        {
            return uniquifier
                .UniquifyAsParts(emailAddressParts)
                ?.ToEmailAddress();
        }
    }
}
