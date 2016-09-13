using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    public class GetDbInitScriptQuery
    {
        public string DbName { get; set; }
        public string Path { get; set; }
        public DbConnectionInfo DbAccount { get; set; }
    }
}
