using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public interface IPreAuthPipeline
    {
    }

    public enum PreAuthResult
    {
        Continue,
        Failed, // why? validation erorr? throw?
        AlreadyAuthenticated 
    }

    public class LockoutPreAuthPipeline
    {
        public Task RunAsync(
            object command, 
            IUserAreaDefinition definition,
            object authMethod
            )
        {
            return Task.CompletedTask;
        }
    }
}
