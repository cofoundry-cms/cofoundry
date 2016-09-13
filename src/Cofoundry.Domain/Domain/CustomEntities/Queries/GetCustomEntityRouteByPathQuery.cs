using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRouteByPathQuery : IQuery<CustomEntityRoute>
    {
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        public int? CustomEntityId { get; set; }

        public int? LocaleId { get; set; }

        public string UrlSlug { get; set; }
    }
}
