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
    public class PageRenderDetailsMapper : IPageRenderDetailsMapper
    {
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;

        public PageRenderDetailsMapper(
            IPageTemplateMicroSummaryMapper pageTemplateMapper
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
        public PageRenderDetails Map(
            PageVersion dbPageVersion
            )
        {
            var page = Mapper.Map<PageRenderDetails>(dbPageVersion);
            page.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);

            return page;
        }
    }
}
