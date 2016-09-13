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
    public class MergeSiteKeywordsValueResolver : IMemberValueResolver<object, object,string, string>
    {
        private readonly ISiteKeywordService _siteKeywordService;

        public MergeSiteKeywordsValueResolver(
            ISiteKeywordService siteKeywordService
            )
        {
            _siteKeywordService = siteKeywordService;
        }

        public string Resolve(object source, object destination, string sourceMember, string destMember, ResolutionContext context)
        {
            return _siteKeywordService.Merge(sourceMember);
        }
    }
}
