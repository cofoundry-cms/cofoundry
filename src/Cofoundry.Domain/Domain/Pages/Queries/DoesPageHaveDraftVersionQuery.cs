using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class DoesPageHaveDraftVersionQuery : IQuery<bool>
    {
        public DoesPageHaveDraftVersionQuery() { }

        public DoesPageHaveDraftVersionQuery(int id)
        {
            PageId = id;
        }

        [Required]
        [PositiveInteger]
        public int PageId { get; set; }
    }
}
