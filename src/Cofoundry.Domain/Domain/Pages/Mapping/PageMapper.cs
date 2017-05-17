using AutoMapper;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageMapper : IPageMapper
    {
        private readonly IPageTemplateMapper _pageTemplateMapper;

        public PageMapper(
            IPageTemplateMapper pageTemplateMapper
            )
        {
            _pageTemplateMapper = pageTemplateMapper;
        }

        /// <summary>
        /// Maps the basic properties on a PageRenderDetails.
        /// </summary>
        /// <remarks>
        /// This isn't a very fully featured map function and will likey
        /// be reworked later on.
        /// </remarks>
        public PageRenderDetails MapRenderDetails(
            PageVersion dbPageVersion
            )
        {
            var page = Mapper.Map<PageRenderDetails>(dbPageVersion);
            page.Template = _pageTemplateMapper.MapMicroSummary(dbPageVersion.PageTemplate);

            return page;
        }
    }
}
