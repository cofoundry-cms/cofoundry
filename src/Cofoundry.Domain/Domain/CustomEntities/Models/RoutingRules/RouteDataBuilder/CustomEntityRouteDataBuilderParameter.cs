using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The arguments passed into ICustomEntityRouteDataBuilder.BuildAsync which
    /// can be used to create additional route data. Additional route data should be 
    /// added by adding values to the AdditionalRoutingData collection.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type (ICustomEntityDataModel) of the custom entity type.</typeparam>
    public class CustomEntityRouteDataBuilderParameter<TDataModel>
        where TDataModel : ICustomEntityDataModel
    {
        private readonly CustomEntityVersionRoute _customEntityVersionRoute;
        private readonly CustomEntityRoute _customEntityRoute;
        private readonly TDataModel _dataModel;
        
        public CustomEntityRouteDataBuilderParameter(
            CustomEntityRoute customEntityRoute,
            CustomEntityVersionRoute customEntityVersionRoute,
            TDataModel dataModel
            )
        {
            if (customEntityRoute == null) throw new ArgumentNullException(nameof(customEntityRoute));
            if (customEntityVersionRoute == null) throw new ArgumentNullException(nameof(customEntityVersionRoute));
            if (dataModel == null) throw new ArgumentNullException(nameof(dataModel));

            _customEntityRoute = customEntityRoute;
            _customEntityVersionRoute = customEntityVersionRoute;
            _dataModel = dataModel;
        }

        /// <summary>
        /// The database id of the custom entity record being mapped.
        /// </summary>
        public int CustomEntityId
        {
            get { return _customEntityRoute.CustomEntityId; }
        }

        /// <summary>
        /// The database id of the custom entity version record being mapped.
        /// </summary>
        public int CustomEntityVersionId
        {
            get { return _customEntityVersionRoute.VersionId; }
        }

        /// <summary>
        /// The locale of the custom entity record being mapped (can be null).
        /// </summary>
        public ActiveLocale Locale
        {
            get { return _customEntityRoute.Locale; }
        }

        /// <summary>
        /// The url slug identifier of the custom entity record being mapped.
        /// </summary>
        public string UrlSlug
        {
            get { return _customEntityRoute.UrlSlug; }
        }

        /// <summary>
        /// The data model of the custom entity record being mapped, mapped from unstructured
        /// data in the database.
        /// </summary>
        public TDataModel DataModel
        {
            get { return _dataModel; }
        }

        /// <summary>
        /// Collection of additional routing data from the CustomEntityVersionRoute.
        /// Your ICustomEntityRouteDataBuilder implementation should add any values
        /// to this collection that you wish to use during routing.
        /// </summary>
        public Dictionary<string, string> AdditionalRoutingData
        {
            get { return _customEntityVersionRoute.AdditionalRoutingData; }
        }
    }
}
