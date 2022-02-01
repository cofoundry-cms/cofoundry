namespace Cofoundry.Web.Identity
{
    public interface ICompleteAccountVerificationViewModel
    {
        /// <summary>
        /// The token used to verify the request.
        /// </summary>
        string Token { get; set; }
    }
}