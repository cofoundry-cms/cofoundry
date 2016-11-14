using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageTemplateSectionMapper
    {
        private readonly IQueryExecutor _queryExecutor;

        public PageTemplateSectionMapper(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

    }
}
