using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageTemplateDetailsByIdQuery : IQuery<PageTemplateDetails>
    {
        public GetPageTemplateDetailsByIdQuery()
        {
        }

        public GetPageTemplateDetailsByIdQuery(int pageTemplateId)
        {
            PageTemplateId = pageTemplateId;
        }

        public int PageTemplateId { get; set; }
    }
}
