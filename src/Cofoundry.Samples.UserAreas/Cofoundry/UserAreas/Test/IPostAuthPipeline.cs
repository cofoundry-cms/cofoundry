using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public interface IPostAuthPipeline
    {
        bool CanRun(IUserAreaDefinition userArea);
    }

    public class LockoutPostAuthPipeline
    {
        public Task RunAsync(AuthPipelineContext context)
        {
            return Task.CompletedTask;
        }
    }

    public class PasswordGeneratablePostAuthPipeline
    {
        public PasswordGeneratablePostAuthPipeline()
        {

        }
        public Task RunAsync(AuthPipelineContext context)
        {
            // 
            return Task.CompletedTask;
        }
    }
}
