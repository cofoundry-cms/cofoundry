﻿using System.Text.RegularExpressions;

namespace Cofoundry.Core.Data.SimpleDatabase;

internal static class SqlHelper
{
    /// <summary>
    /// Splits a MS SqlServer script into batches, removing comments
    /// and splitting scripts on the GO keyword. Empty batches are 
    /// automatically removed.
    /// </summary>
    /// <param name="sql">SQL Script to split.</param>
    public static IEnumerable<string> SplitIntoBatches(string sql)
    {
        ArgumentNullException.ThrowIfNull(sql);

        // Remove comments
        sql = Regex.Replace(sql, "--.*", string.Empty);

        // Split go statements into batches
        var sqlBatches = Regex.Split(sql, @"^\s*(?:go){1}\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
            .Where(s => !string.IsNullOrWhiteSpace(s));

        return sqlBatches;
    }
}
