using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Domain
{
    public static class PublishStatusQueryExtensions
    {
        /// <summary>
        /// Turns the PublishStatusQuery used to query an entity into a PublishStatusQuery
        /// that can be used to query dependent entities. E.g. PublishStatusQuery.SpecificVersion
        /// cannot be used to query a dependent entity and so PublishStatusQuery.Latest is 
        /// used instead.
        /// </summary>
        /// <remarks>
        /// When working with child entities, the PublishStatusQuery we apply to
        /// them is not neccessarily the status used to query the parent. If we are 
        /// loading a page using the Draft status, then we cannot expect that all 
        /// dependencies should have a draft version, so we re-write it to Latest.
        /// The same applies if we're loading a specific version.
        /// </remarks>
        public static PublishStatusQuery ToRelatedEntityQueryStatus(this PublishStatusQuery publishStatusQuery)
        {
            if (publishStatusQuery == PublishStatusQuery.Draft || publishStatusQuery == PublishStatusQuery.SpecificVersion)
            {
                publishStatusQuery = PublishStatusQuery.Latest;
            }

            return publishStatusQuery;
        }
    }
}
