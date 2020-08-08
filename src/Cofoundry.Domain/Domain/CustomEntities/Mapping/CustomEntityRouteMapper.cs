using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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
        public CustomEntityRoute Map(
            CustomEntity dbCustomEntity, 
            ActiveLocale locale
            )
        {
            if (dbCustomEntity == null) throw new ArgumentNullException(nameof(dbCustomEntity));
            if (dbCustomEntity.CustomEntityVersions == null) throw new ArgumentNullException(nameof(dbCustomEntity.CustomEntityVersions));

            var route = new CustomEntityRoute()
            {
                CustomEntityDefinitionCode = dbCustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbCustomEntity.CustomEntityId,
                UrlSlug = dbCustomEntity.UrlSlug,
                Locale = locale,
                PublishDate = DbDateTimeMapper.AsUtc(dbCustomEntity.PublishDate),
                PublishStatus = dbCustomEntity.PublishStatusCode == PublishStatusCode.Published ? PublishStatus.Published : PublishStatus.Unpublished,
                Ordering = dbCustomEntity.Ordering
            };

            bool hasLatestPublishVersion = false;
            route.Versions = new List<CustomEntityVersionRoute>();

            foreach (var dbVersion in dbCustomEntity
                .CustomEntityVersions
                .OrderByLatest())
            {
                var version = new CustomEntityVersionRoute()
                {
                    CreateDate = DbDateTimeMapper.AsUtc(dbVersion.CreateDate),
                    Title = dbVersion.Title,
                    VersionId = dbVersion.CustomEntityVersionId,
                    WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId
                };

                if (!hasLatestPublishVersion && version.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    version.IsLatestPublishedVersion = true;
                    hasLatestPublishVersion = true;
                }
                route.Versions.Add(version);
            }

            route.HasDraftVersion = route.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Draft);
            route.HasPublishedVersion = route.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);

            return route;
        }
    }
}
