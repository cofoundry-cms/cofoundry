using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityRoute objects.
    /// </summary>
    public class CustomEntityRouteMapper : ICustomEntityRouteMapper
    {
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;

        public CustomEntityRouteMapper(
            ICustomEntityDataModelMapper customEntityDataModelMapper
            )
        {
            _customEntityDataModelMapper = customEntityDataModelMapper;
        }

        /// <summary>
        /// Maps an EF CustomEntity record from the db into a CustomEntityRoute object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbCustomEntity">CustomEntity record from the database.</param>
        /// <param name="locale">Locale to map to the object.</param>
        /// <param name="routingDataProperties">Collection of data properties to map to the routing parameters collection.</param>
        public CustomEntityRoute Map(
            CustomEntity dbCustomEntity, 
            ActiveLocale locale,
            IEnumerable<PropertyInfo> routingDataProperties
            )
        {
            if (dbCustomEntity == null) throw new ArgumentNullException(nameof(dbCustomEntity));

            var route = new CustomEntityRoute()
            {
                CustomEntityDefinitionCode = dbCustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbCustomEntity.CustomEntityId,
                UrlSlug = dbCustomEntity.UrlSlug,
                Locale = locale,
                PublishDate = dbCustomEntity.PublishDate,
                PublishStatus = dbCustomEntity.PublishStatusCode == PublishStatusCode.Published ? PublishStatus.Published : PublishStatus.Unpublished
            };

            var versions = new List<CustomEntityVersionRoute>();
            route.Versions = versions;

            foreach (var dbVersion in dbCustomEntity.CustomEntityVersions)
            {
                var version = new CustomEntityVersionRoute()
                {
                    CreateDate = dbVersion.CreateDate,
                    Title = dbVersion.Title,
                    VersionId = dbVersion.CustomEntityVersionId,
                    WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId
                };
                versions.Add(version);

                if (routingDataProperties.Any())
                {
                    // Parse additional routing data properties
                    var model = _customEntityDataModelMapper.Map(dbCustomEntity.CustomEntityDefinitionCode, dbVersion.SerializedData);
                    foreach (var routingDataProperty in routingDataProperties)
                    {
                        version.AdditionalRoutingData.Add(routingDataProperty.Name, Convert.ToString(routingDataProperty.GetValue(model)));
                    }
                }
            }

            return route;
        }
    }
}
