namespace Cofoundry.Web.Identity
{
    public interface ICompleteAccountRecoveryViewModel
    {
        /// <summary>
        /// The value to set as the new account password. The password will go through 
        /// additional validation depending on the password policy configuration.
        /// </summary>
        string NewPassword { get; set; }

        /// <summary>
        /// The token used to verify the request.
        /// </summary>
        string Token { get; set; }
    }
}