using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Registers all the default admin api routes. Instead of using attribute
    /// routing we instead use the Cofoundry routeBuilder.ForAdminApiController
    /// extension methods which ensures the routes are registered
    /// using the correct admin route, which can be changed via config.
    /// </summary>
    public class AdminApiRouteRegistration : IOrderedRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            MapAccountApiRoutes(routeBuilder);
            MapCustomEntityApiRoutes(routeBuilder);
            MapDashboardApiRoutes(routeBuilder);
            MapDocumentApiRoutes(routeBuilder);
            MapDynamicDataApiRoutes(routeBuilder);
            MapImageApiRoutes(routeBuilder);
            MapLocaleApiRoutes(routeBuilder);
            MapPageDirectoryApiRoutes(routeBuilder);
            MapPageApiRoutes(routeBuilder);
            MapPageTemplateApiRoutes(routeBuilder);
            MapRolesApiRoutes(routeBuilder);
            MapSettingsApiRoutes(routeBuilder);
            MapSetupApiRoutes(routeBuilder);
            MapUserApiRoutes(routeBuilder);
        }

        private static void MapAccountApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<AccountApiController>("account")
                .MapGet()
                .MapPatch()
                .MapPut("Password", "PutPassword");
        }

        private static void MapCustomEntityApiRoutes(IRouteBuilder routeBuilder)
        {
            var customEntityIdRoute = "{customEntityId:int}";

            routeBuilder
                .ForAdminApiController<CustomEntitiesApiController>("custom-entities")
                .MapGet()
                .MapGetById(customEntityIdRoute)
                .MapPost()
                .MapPut("ordering", "PutOrdering")
                .MapPut(customEntityIdRoute + "/url", "PutCustomEntityUrl")
                .MapDelete(customEntityIdRoute)
                .MapPost(customEntityIdRoute + "/duplicate", "PostDuplicate")
                ;

            routeBuilder
                .ForAdminApiController<CustomEntityDataModelSchemaApiController>("custom-entity-data-model-schemas")
                .MapGet()
                .MapGet("{customEntityDefinitionCode}", "GetDataModelSchema")
                ;

            var customEntityDefinitionCodeRoute = "{customEntityDefinitionCode}";

            routeBuilder
                .ForAdminApiController<CustomEntityDefinitionsApiController>("custom-entity-definitions")
                .MapGet()
                .MapGetById(customEntityDefinitionCodeRoute)
                .MapGet(customEntityDefinitionCodeRoute + "/routes", "GetCustomEntityRoutes")
                .MapGet(customEntityDefinitionCodeRoute + "/data-model-schema", "GetDataModelSchema")
                .MapGet(customEntityDefinitionCodeRoute + "/custom-entities", "GetCustomEntities")
                ;

            routeBuilder
                .ForAdminApiController<CustomEntityRoutingRulesApiController>("custom-entity-routing-rules")
                .MapGet()
                ;

            var customEntityVersionPageBlockIdRoute = "{customEntityVersionPageBlockId:int}";

            routeBuilder
                .ForAdminApiController<CustomEntityVersionPageBlocksApiController>("custom-entity-version-page-blocks")
                .MapGet(customEntityVersionPageBlockIdRoute)
                .MapPost()
                .MapPut(customEntityVersionPageBlockIdRoute)
                .MapDelete(customEntityVersionPageBlockIdRoute)
                .MapPut(customEntityVersionPageBlockIdRoute + "/move-up", "MoveUp")
                .MapPut(customEntityVersionPageBlockIdRoute + "/move-down", "MoveDown")
                ;

            routeBuilder
                .ForAdminApiController<CustomEntityVersionsApiController>("custom-entities/{customEntityId:int}/versions")
                .MapGet()
                .MapPost()
                .MapPut("draft", "PutDraft")
                .MapDelete("draft", "DeleteDraft")
                .MapPatch("draft/publish", "Publish")
                .MapPatch("published/unpublish", "UnPublish")
                ;
        }

        private static void MapDashboardApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<DashboardApiController>("dashboard")
                .MapGet()
                ;
        }

        private static void MapDocumentApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<DocumentFileTypesApiController>("document-file-types")
                .MapGet()
                ;

            var documentAssetIdRoute = "{documentAssetId:int}";

            routeBuilder
                .ForAdminApiController<DocumentsApiController>("documents")
                .MapGet()
                .MapGetById(documentAssetIdRoute)
                .MapPost()
                .MapPut(documentAssetIdRoute)
                .MapDelete(documentAssetIdRoute)
                ;
        }

        private static void MapDynamicDataApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<NestedDataModelSchemaApiController>("nested-data-model-schemas")
                .MapGet("{dataModelName}")
                ;
        }

        private static void MapImageApiRoutes(IRouteBuilder routeBuilder)
        {
            var imageAssetIdRoute = "{imageAssetId:int}";

            routeBuilder
                .ForAdminApiController<ImagesApiController>("images")
                .MapGet()
                .MapGetById(imageAssetIdRoute)
                .MapPost()
                .MapPut(imageAssetIdRoute)
                .MapDelete(imageAssetIdRoute)
                ;
        }

        private static void MapLocaleApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<LocalesApiController>("locales")
                .MapGet()
                ;
        }

        private static void MapPageDirectoryApiRoutes(IRouteBuilder routeBuilder)
        {
            var pageDirectoryIdRoute = "{pageDirectoryId:int}";

            routeBuilder
                .ForAdminApiController<PageDirectoriesApiController>("page-directories")
                .MapGet()
                .MapGet("tree", "GetTree")
                .MapGetById(pageDirectoryIdRoute)
                .MapPost()
                .MapPatch(pageDirectoryIdRoute)
                .MapDelete(pageDirectoryIdRoute)
                ;
        }

        private static void MapPageApiRoutes(IRouteBuilder routeBuilder)
        {

            routeBuilder
                .ForAdminApiController<PageBlockTypesApiController>("page-block-types")
                .MapGet()
                .MapGetById("{pageBlockTypeId:int}")
                ;

            var pageIdRoute = "{pageId:int}";

            routeBuilder
                .ForAdminApiController<PagesApiController>("pages")
                .MapGet()
                .MapGetById(pageIdRoute)
                .MapPost()
                .MapPatch(pageIdRoute)
                .MapPut(pageIdRoute + "/url", "PutPageUrl")
                .MapDelete(pageIdRoute)
                .MapPost(pageIdRoute + "/duplicate", "PostDuplicate")
                ;

            var pageVersionBlockIdRoute = "{pageVersionBlockId:int}";

            routeBuilder
                .ForAdminApiController<PageVersionRegionBlocksApiController>("page-version-region-blocks")
                .MapGet(pageVersionBlockIdRoute)
                .MapPost()
                .MapPut(pageVersionBlockIdRoute)
                .MapDelete(pageVersionBlockIdRoute)
                .MapPut(pageVersionBlockIdRoute + "/move-up", "MoveUp")
                .MapPut(pageVersionBlockIdRoute + "/move-down", "MoveDown")
                ;

            routeBuilder
                .ForAdminApiController<PageVersionsApiController>("pages/{pageId:int}/versions")
                .MapGet()
                .MapPost()
                .MapPatch("draft", "PatchDraft")
                .MapDelete("draft")
                .MapPatch("draft/publish", "Publish")
                .MapPatch("published/unpublish", "UnPublish")
                ;

        }

        private static void MapPageTemplateApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<PageTemplatesApiController>("page-templates")
                .MapGet()
                .MapGetById("{id:int}")
                ;
        }

        private static void MapRolesApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<PermissionsApiController>("permissions")
                .MapGet()
                ;

            var roleIdRoute = "{roleId:int}";

            routeBuilder
                .ForAdminApiController<RolesApiController>("roles")
                .MapGet()
                .MapGetById(roleIdRoute)
                .MapPost()
                .MapPatch(roleIdRoute)
                .MapDelete(roleIdRoute)
                ;
        }

        private static void MapSettingsApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<CacheApiController>("cache")
                .MapDelete()
                ;

            var generalRoute = "GeneralSite";
            var settingsRoute = "Seo";

            routeBuilder
                .ForAdminApiController<SettingsApiController>("settings")
                .MapGet(generalRoute, "GetGeneralSiteSettings")
                .MapGet(settingsRoute, "GetSeoSettings")
                .MapPatch(generalRoute, "PatchGeneralSiteSettings")
                .MapPatch(settingsRoute, "PatchSeoSettings")
                ;
        }

        private static void MapSetupApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<SetupApiController>("setup")
                .MapPost()
                ;
        }

        private static void MapUserApiRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder
                .ForAdminApiController<UserAreasApiController>("user-areas")
                .MapGet()
                ;

            var userIdRoute = "{userId:int}";

            routeBuilder
                .ForAdminApiController<UsersApiController>("users")
                .MapGet()
                .MapGetById(userIdRoute)
                .MapPost()
                .MapPatch(userIdRoute)
                .MapDelete(userIdRoute)
                ;
        }

    }
}
