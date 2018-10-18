using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummariesByDefinitionCodeQuery : IQuery<ICollection<CustomEntityRenderSummary>>
    {
        public GetCustomEntityRenderSummariesByDefinitionCodeQuery()
        {
        }

        public GetCustomEntityRenderSummariesByDefinitionCodeQuery(
            string customEntityDefinitionCode,
            PublishStatusQuery workflowStatus = PublishStatusQuery.Published
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            PublishStatus = workflowStatus;
        }

        /// <summary>
        /// Required. The definition code of the custom entity to filter on.
        /// </summary>
        [Required]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Used to determine which version of the custom entities to include data for. This 
        /// defaults to Published, meaning that only published custom entities will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }
    }
}
