using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsPageTemplateSectionNameUniqueQuery : IQuery<bool>
    {
        public int? PageTemplateSectionId { get; set; }

        public string Name { get; set; }

        [Required]
        public int PageTemplateId { get; set; }
    }
}
