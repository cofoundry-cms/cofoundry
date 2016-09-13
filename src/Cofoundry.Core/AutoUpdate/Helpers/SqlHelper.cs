using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    internal static class SqlHelper
    {
        public static IEnumerable<string> SplitIntoBatches(string sql)
        {
            // Remove comments
            sql = Regex.Replace(sql, "--.*", string.Empty);

            // Split go statements into batches
            var sqlBatches = Regex.Split(sql, @"^go\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            return sqlBatches;
        }
    }
}
