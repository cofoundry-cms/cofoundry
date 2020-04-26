using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    public class TestController : Controller
    {
        private readonly IAdvancedContentRepository _contentRepository;

        public TestController(
            IAdvancedContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        [Route("/test-admin/api/pets")]
        public IActionResult Pets()
        {
            var result = new object[]
            {
                new { Id = 1, Title = "Dog" },
                new { Id = 2, Title = "Cat" },
                new { Id = 3, Title = "Llama" }
            };

            return Json(result);
        }


        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            var user = await _contentRepository
                .WithElevatedPermissions()
                .Users()
                .GetById(2)
                .AsMicroSummaryAsync();

            var image = await _contentRepository
                .ImageAssets()
                .GetById(2)
                .AsRenderDetailsAsync();

            var page = await _contentRepository
                .Pages()
                .Search()
                .AsRenderSummariesAsync(new SearchPageRenderSummariesQuery()
                {
                    PageSize = 20,
                    PageDirectoryId = 2
                });

            var page2 = await _contentRepository
                .Pages()
                .GetById(1)
                .AsRenderSummaryAsync();

            // Perhaps NotFound should look like this:
            await _contentRepository
                .Pages()
                .NotFound()
                .GetByPathAsync(new GetNotFoundPageRouteByPathQuery());

            var regions = await _contentRepository
                .Pages()
                .Versions()
                .Regions()
                .Blocks()
                .GetById(2)
                .AsRenderDetailsAsync(PublishStatusQuery.Latest);

            await _contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery());
            //.PublishAsync(new PublishPageCommand() { PageId = 2 });

            var isUnique = await _contentRepository
                .Users()
                .IsUsernameUniqueAsync(new IsUsernameUniqueQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Username = "test@cofoundry.org"
                });

            int userId;

            using (var scope = _contentRepository.Transactions().CreateScope())
            {
                var adminRole = await _contentRepository
                    .Roles()
                    .GetByCode(SuperAdminRole.SuperAdminRoleCode)
                    .AsDetailsAsync();

                userId = await _contentRepository
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(new AddUserCommand()
                    {
                        Email = Guid.NewGuid().ToString() + "@cofoundry.org",
                        Password = "badpassword",
                        UserAreaCode = CofoundryAdminUserArea.AreaCode,
                        RoleId = adminRole.RoleId
                    });

                //await _contentRepository
                //    .WithElevatedPermissions()
                //    .ImageAssets()
                //    .DeleteAsync(2);

                await _contentRepository
                    .CustomEntities()
                    .Definitions()
                    .GetByCode(BlogPostCustomEntityDefinition.DefinitionCode)
                    .AsSummaryAsync();

                await _contentRepository
                    .CustomEntities()
                    .DataModelSchemas()
                    .GetByCustomEntityDefinitionCode(BlogPostCustomEntityDefinition.DefinitionCode)
                    .AsDetailsAsync();

                await _contentRepository
                    .CustomEntities()
                    .GetById(1)
                    .AsRenderSummaryAsync();

                var permissions = await _contentRepository
                    .Roles()
                    .Permissions()
                    .GetAll()
                    .AsIPermissionAsync();

                var directoryTree = await _contentRepository
                    .PageDirectories()
                    .GetAll()
                    .AsTreeAsync();

                var rewriteRules = await _contentRepository
                    .RewriteRules()
                    .GetAll()
                    .AsSummariesAsync();

                var blockTypes = await _contentRepository
                    .PageBlockTypes()
                    .GetAll()
                    .AsSummariesAsync();

                await scope.CompleteAsync();
            }

            await _contentRepository
                .WithElevatedPermissions()
                .Users()
                .DeleteUserAsync(userId);

            return View();
        }
    }
}
