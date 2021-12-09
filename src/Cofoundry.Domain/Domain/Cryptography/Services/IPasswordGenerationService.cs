namespace Cofoundry.Domain
{
    /// <summary>
    /// Generates cryptographically random passwords. 
    /// </summary>
    public interface IPasswordGenerationService
    {
        /// <summary>
        /// Generates a cryptographically random password using the default
        /// password length and characterset.
        /// </summary>
        string Generate();

        /// <summary>
        /// Generates a cryptographically random password of the specified
        /// length.
        /// </summary>
        /// <param name="passwordLength">
        /// The password length, must be 6 characters or more.
        /// </param>
        string Generate(int passwordLength);
    }
}
