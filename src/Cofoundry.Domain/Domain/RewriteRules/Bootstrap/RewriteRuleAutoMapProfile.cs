using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class RewriteRuleAutoMapProfile : Profile
    {
        public RewriteRuleAutoMapProfile()
        {
            CreateMap<RewriteRule, RewriteRuleSummary>();
        }
    }
}
