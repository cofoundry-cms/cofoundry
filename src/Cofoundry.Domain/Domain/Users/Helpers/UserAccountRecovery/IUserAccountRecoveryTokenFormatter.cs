namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to create a single token that identifies
    /// and authenticates an account recovery request.
    /// </summary>
    public interface IUserAccountRecoveryTokenFormatter
    {
        /// <summary>
        /// Formats a new token from the specified <paramref name="tokenParts"/>.
        /// </summary>
        /// <param name="tokenParts">
        /// The data to use to create the token, which must not be null
        /// or contain empty data.
        /// </param>
        public string Format(UserAccountRecoveryTokenParts tokenParts);

        /// <summary>
        /// Parses a token back to its consituent parts. If the token
        /// could not be parsed or if any parts are not present then
        /// <see langword="null"/> is returned.
        /// </summary>
        /// <param name="token">
        /// The token to parse. Can be <see langword="null"/> 
        /// which will return a <see langword="null"/> result.
        /// </param>
        public UserAccountRecoveryTokenParts Parse(string token);
    }
}
