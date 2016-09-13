using AutoMapper;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateCustomEntityTypeValueResolver : IMemberValueResolver<PageTemplate, PageTemplateMicroSummary, string, Type>
    {
        private readonly PageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public PageTemplateCustomEntityTypeValueResolver(
            PageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
            )
        {
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        }

        public Type Resolve(PageTemplate source, PageTemplateMicroSummary destination, string sourceMember, Type destMember, ResolutionContext context)
        {
            return _pageTemplateCustomEntityTypeMapper.Map(sourceMember);
        }
    }
}
