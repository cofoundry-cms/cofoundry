using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchPageSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<PageSummary>>
    {
        [Display(Name = "Tags")]
        public string Tags { get; set; }

        [Display(Name = "Status")]
        public WorkFlowStatus? WorkFlowStatus { get; set; }

        [Display(Name = "Market")]
        public int? LocaleId { get; set; }

        [Display(Name = "Directory")]
        public int? WebDirectoryId { get; set; }

        [Display(Name = "Templates")]
        public int? PageTemplateId { get; set; }

        [Display(Name = "Page group")]
        public int? PageGroupId { get; set; }
    }
}
