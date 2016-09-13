using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An enum for rule priorities so standard values can be used and changed internally 
    /// if need be.
    /// </summary>
    public enum RoutingRulePriority
    {
        /// <summary>
        /// Process this rule first before any others
        /// </summary>
        First = 0,

        /// <summary>
        /// Run this rule before normal rules but after anything marked as 'First'
        /// </summary>
        Earlier = 50,

        /// <summary>
        /// Give no intended priority to this rule
        /// </summary>
        Normal = 100,

        /// <summary>
        /// Run this rule after normal rules but before anything marked as 'Last'
        /// </summary>
        Later = 150,

        /// <summary>
        /// Make sure this rule runs after all others
        /// </summary>
        Last = 200
    }
}
