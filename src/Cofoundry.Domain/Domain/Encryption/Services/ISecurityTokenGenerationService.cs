using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Generates random security tokens that can be useful for a variety of tasks.
    /// </summary>
    public interface ISecurityTokenGenerationService
    {
        /// <summary>
        /// Generates a unique and random security token that can be used to verify
        /// a request without a username and password, e.g. for a password reset link
        /// </summary>
        string Generate();
    }
}
