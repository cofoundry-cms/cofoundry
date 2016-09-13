using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsCustomEntityPathUniqueQuery : IQuery<bool>
    {
        public int? CustomEntityId { get; set; }

        public string UrlSlug { get; set; }
        
        public int? LocaleId { get; set; }

        [Required]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
