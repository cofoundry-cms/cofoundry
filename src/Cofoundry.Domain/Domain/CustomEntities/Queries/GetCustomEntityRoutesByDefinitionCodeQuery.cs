using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRoutesByDefinitionCodeQuery : IQuery<IEnumerable<CustomEntityRoute>>
    {
        public GetCustomEntityRoutesByDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
