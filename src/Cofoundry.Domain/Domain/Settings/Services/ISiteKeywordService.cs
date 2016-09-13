using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using AutoMapper;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Aggregates given keywords with the site-wide pre-defined meta keywords
    /// </summary>
    public interface ISiteKeywordService
    {
        /// <summary>
        /// Merges the specified string of keywords with the global site keywords
        /// </summary>
        string Merge(string keywordsToMerge);
    }
}
