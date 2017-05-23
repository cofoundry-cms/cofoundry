using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Core.Caching;
using Cofoundry.Web.WebApi;
using Cofoundry.Core.Validation;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("cache")]
    public class CacheApiController : BaseAdminApiController
    {
        #region private member variables

        private readonly IObjectCacheFactory _objectCacheFactory;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public CacheApiController(
            IApiResponseHelper apiResponseHelper,
            IObjectCacheFactory objectCacheFactory
            )
        {
            _objectCacheFactory = objectCacheFactory;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region commands

        /// <summary>
        /// Admin remote access method to clear the
        /// data cache in case we run into a caching issue in a live
        /// deployment
        /// </summary>
        [HttpDelete]
        public IActionResult Delete()
        {
            _objectCacheFactory.Clear();

            return _apiResponseHelper.SimpleCommandResponse(this, Enumerable.Empty<ValidationError>());
        }

        #endregion

        #endregion
    }
}