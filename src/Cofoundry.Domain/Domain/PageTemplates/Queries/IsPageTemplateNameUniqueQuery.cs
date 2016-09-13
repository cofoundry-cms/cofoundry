using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsPageTemplateNameUniqueQuery : IQuery<bool>
    {
        public int? PageTemplateId { get; set; }

        public string Name { get; set; }
    }
}
