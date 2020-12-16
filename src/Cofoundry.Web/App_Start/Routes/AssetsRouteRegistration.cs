using System;
using System.Collections.Generic;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web
{
    public class AssetsRouteRegistration : IOrderedRouteRegistration, IRunAfterRouteRegistration
    {
        private readonly DocumentAssetsSettings _documentAssetsSettings;
        private readonly ImageAssetsSettings _imageAssetsSettings;

        public AssetsRouteRegistration(
            DocumentAssetsSettings documentAssetsSettings,
            ImageAssetsSettings imageAssetsSettings
            )
        {
            _documentAssetsSettings = documentAssetsSettings;
            _imageAssetsSettings = imageAssetsSettings;
        }

        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public ICollection<Type> RunAfter => new Type[] { typeof(RobotsRouteRegistration) };

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            if (!_imageAssetsSettings.Disabled)
            {
                routeBuilder.MapControllerRoute(
                    "Cofoundry_ImageAsset",
                    "assets/images/{imageAssetId}-{fileStamp}-{verificationToken}/{fileName}.{extension}",
                    new { controller = "CofoundryAssets", action = "Image" },
                    new { imageAssetId = @"\d+", fileStamp = @"\d+" });

                if (_imageAssetsSettings.EnableCompatibilityRoutesFor0_4)
                {
                    // These old route are to support pre v0.5 asset routes.

                    routeBuilder.MapControllerRoute(
                        "Cofoundry_ImageAsset_OldPath",
                        "assets/images/{assetId}_{fileName}.{extension}",
                        new { controller = "CofoundryAssets", action = "Image_OldPath" },
                        new { assetId = @"\d+" });

                    routeBuilder.MapControllerRoute(
                        "Cofoundry_DocumentAsset_Download_OldPath",
                        "assets/files/download/{assetId}_{fileName}.{extension}",
                        new { controller = "CofoundryAssets", action = "FileDownload_OldPath" },
                        new { assetId = @"\d+" });
                }
            }

            if (!_documentAssetsSettings.Disabled)
            {
                routeBuilder.MapControllerRoute(
                    "Cofoundry_DocumentAsset",
                    "assets/documents/{documentAssetId}-{fileStamp}-{verificationToken}/{fileName}.{extension}",
                    new { controller = "CofoundryAssets", action = "Document" },
                    new { documentAssetId = @"\d+", fileStamp = @"\d+" });

                routeBuilder.MapControllerRoute(
                    "Cofoundry_DocumentAsset_Download",
                    "assets/documents/download/{documentAssetId}-{fileStamp}-{verificationToken}/{fileName}.{extension}",
                    new { controller = "CofoundryAssets", action = "DocumentDownload" },
                    new { documentAssetId = @"\d+", fileStamp = @"\d+" });

                if (_documentAssetsSettings.EnableCompatibilityRoutesFor0_4)
                {
                    // These old route are to support pre v0.5 asset routes.

                    routeBuilder.MapControllerRoute(
                        "Cofoundry_DocumentAsset_OldPath",
                        "assets/files/{assetId}_{fileName}.{extension}",
                        new { controller = "CofoundryAssets", action = "File_OldPath" },
                        new { assetId = @"\d+" });
                }

            }
        }
    }
}
