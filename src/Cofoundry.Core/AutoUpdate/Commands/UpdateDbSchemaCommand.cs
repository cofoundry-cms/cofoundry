using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    public class UpdateDbSchemaCommand
    {
        public string Sql { get; set; }
        public int Version { get; set; }
    }
}
