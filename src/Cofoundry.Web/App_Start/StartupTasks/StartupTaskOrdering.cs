using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Some sensible defaults for the numerical ordering value in an
    /// IStartupTask.
    /// </summary>
    public enum StartupTaskOrdering
    {
        Normal = 0,
        First = -400,
        Early = -200,
        Late = 200,
        Last = 400
    }
}