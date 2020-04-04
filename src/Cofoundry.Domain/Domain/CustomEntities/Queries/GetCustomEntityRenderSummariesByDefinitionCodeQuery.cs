using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to retreive all custom entites of a specific type, projected as a
    /// CustomEntityRenderSummary, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.    
    /// </summary>
    public class GetCustomEntityRenderSummariesByDefinitionCodeQuery : IQuery<ICollection<CustomEntityRenderSummary>>
    {
        /// <summary>
        /// Query to retreive all custom entites of a specific type, projected as a
        /// CustomEntityRenderSummary, which is a general-purpose projection with version 
        /// specific data, including a deserialized data model. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.    
        /// </summary>
        public GetCustomEntityRenderSummariesByDefinitionCodeQuery()
        {
        }

        /// <summary>
        /// Query to retreive all custom entites of a specific type, projected as a
        /// CustomEntityRenderSummary, which is a general-purpose projection with version 
        /// specific data, including a deserialized data model. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.    
        /// </summary>
        /// <param name="customEntityDefinitionCode">The definition code of the custom entity to filter on.</param>
        /// <param name="publicStatusQuery">
        /// Used to determine which version of the custom entities to include data for. This 
        /// defaults to Published, meaning that only published custom entities will be returned.
        /// </param>
        public GetCustomEntityRenderSummariesByDefinitionCodeQuery(
            string customEntityDefinitionCode,
            PublishStatusQuery publicStatusQuery = PublishStatusQuery.Published
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            PublishStatus = publicStatusQuery;
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
