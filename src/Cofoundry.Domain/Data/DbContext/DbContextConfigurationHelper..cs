using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class DbContextConfigurationHelper
    {
        /// <summary>
        /// Turns off LazyLoading and adds a console debug logger.
        /// </summary>
        /// <param name="dbContext">dbContext to set defaults on.</param>
        public static void SetDefaults(DbContext dbContext)
        {
            //dbContext.Configuration.LazyLoadingEnabled = false;
            AddConsoleLogger(dbContext);
        }

        /// <summary>
        /// Logs db output to the console if the debugger is attached.
        /// </summary>
        /// <param name="dbContext">dbContext to attach the logger to</param>
        public static void AddConsoleLogger(DbContext dbContext)
        {
            //dbContext.Database.Log = (s) => System.Diagnostics.Debug.Write(s);
        }
    }
}
