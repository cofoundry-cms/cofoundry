using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// A command that executes a sql db script.
    /// </summary>
    public class UpdateDbCommand : IVersionedUpdateCommand
    {
        /// <summary>
        /// The name of the sql file without the .sql extension. For single
        /// object files this will be the name of the object by convention.
        /// </summary>
        public string FileName { get; set; }
        public string Sql { get; set; }
        public int Version { get; set; }
        public DbScriptType ScriptType { get; set; }
        public string Description { get; set; }
    }
}
