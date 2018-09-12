using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web
{
    public class AssetsRouteRegistration : IOrderedRouteRegistration, IRunAfterRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public ICollection<Type> RunAfter => new Type[] { typeof(RobotsRouteRegistration) };

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Cofoundry_ImageAsset",
                "assets/images/{assetId}_{fileName}.{extension}",
                new { controller = "CofoundryAssets", action = "Image" },
                new { assetId = @"\d+" });

            routeBuilder.MapRoute(
                "Cofoundry_DocumentAsset",
                "assets/files/{assetId}_{fileName}.{extension}",
                new { controller = "CofoundryAssets", action = "File" },
                new { assetId = @"\d+" });

            routeBuilder.MapRoute(
                "Cofoundry_DocumentAsset_Download",
                "assets/files/download/{assetId}_{fileName}.{extension}",
                new { controller = "CofoundryAssets", action = "FileDownload" },
                new { assetId = @"\d+" });
        }
    }
}
