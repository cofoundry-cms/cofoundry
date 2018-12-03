using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public interface IAuthPipeline
    {
        bool CanRun(IUserAreaDefinition userArea);
    }

    /// <summary>
    /// In a mixed auth scenario, the flow for 
    /// google auth would be different to password
    /// </summary>
    public enum AuthType
    {
        Password,
        Token,
        Other
    }

    // TODO: Keep thinking about this and sculpting it out.
    public class AuthPipelineContext
    {
        public bool? IsAuthenticated { get; set; }

        public bool? CanLogIn { get; set; }

        public object LoginCommand { get; set; }

        public IUserAreaDefinition UserArea { get; set; }

        public UserSummary User { get; set; }
    }

    public class PasswordAuthPipeline
    {
        public Task<AuthPipelineContext> AuthAsync(
            object loginCommand,
            IUserAreaDefinition context
            )
        {
            var cx = new AuthPipelineContext();

            return Task.FromResult(cx);
        }
    }

    public class FacebookAuthPipeline
    {
        public Task<AuthPipelineContext> AuthAsync(
            object loginCommand,
            IUserAreaDefinition context
            )
        {
            var cx = new AuthPipelineContext();

            return Task.FromResult(cx);
        }
    }
}
