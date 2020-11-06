using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using System.Transactions;
using Cofoundry.Domain.TransactionManager.Default;

namespace Cofoundry.BasicTestSite
{
    public class TestController : Controller
    {
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IDomainRepository _domainRepository;

        public TestController(
            IAdvancedContentRepository contentRepository,
            IDomainRepository domainRepository
            )
        {
            _contentRepository = contentRepository;
            _domainRepository = domainRepository;
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

        [Route("test2")]
        public async Task<IActionResult> Test2()
        {
            await _domainRepository
                .WithElevatedPermissions()
                .ExecuteQueryAsync(new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(""));

            var entity = await _contentRepository
                .WithElevatedPermissions()
                .CustomEntities()
                .GetById(1)
                .AsRenderSummary(PublishStatusQuery.Published)
                .ExecuteAsync();

            var entities = await _contentRepository
                .CustomEntities()
                .GetByDefinitionCode("")
                .AsRenderSummary(PublishStatusQuery.Published)
                .MapItem(e => new
                {
                    e.Title
                })
                .ExecuteAsync();

            var enity2 = await _contentRepository
                .CustomEntities()
                .GetById(1)
                .AsRenderSummary(PublishStatusQuery.Published)
                .Map(e => new Dictionary<string, string>())
                .ExecuteAsync();

            var ids = new int[] { 1 };
            var catCustomEntities = await _contentRepository
                .CustomEntities()
                .GetByIdRange(ids)
                .AsRenderSummaries()
                //.MapItem(i => new { i.UrlSlug, i.Title })
                .MapItem(MapCatAsync)
                .FilterAndOrderByKeys(ids)
                .Map(v => v.OrderBy(v => v.Title))
                .ExecuteAsync();

            var customExecution = await _domainRepository
                .WithQuery(new SearchCustomEntityRenderSummariesQuery())
                .MapItem(b => new { b.CreateDate })
                .ExecuteAsync();

            return Json(entity);
        }

        private Task<CustomEntityRenderSummary> MapCatAsync(CustomEntityRenderSummary entity)
        {
            return Task.FromResult(entity);
        }

        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            var user = await _contentRepository
                .WithElevatedPermissions()
                .Users()
                .GetById(2)
                .AsMicroSummary()
                .ExecuteAsync();

            var image = await _contentRepository
                .ImageAssets()
                .GetById(2)
                .AsRenderDetails()
                .ExecuteAsync();

            var page = await _contentRepository
                .Pages()
                .Search()
                .AsRenderSummaries(new SearchPageRenderSummariesQuery()
                {
                    PageSize = 20,
                    PageDirectoryId = 2
                })
                .ExecuteAsync();

            var page2 = await _contentRepository
                .Pages()
                .GetById(1)
                .AsRenderSummary()
                .ExecuteAsync();

            // Perhaps NotFound should look like this:
            await _contentRepository
                .Pages()
                .NotFound()
                .GetByPath(new GetNotFoundPageRouteByPathQuery())
                .ExecuteAsync();

            var regions = await _contentRepository
                .Pages()
                .Versions()
                .Regions()
                .Blocks()
                .GetById(2)
                .AsRenderDetails(PublishStatusQuery.Latest)
                .ExecuteAsync();

            await _contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery())
                .ExecuteAsync();
            //.PublishAsync(new PublishPageCommand() { PageId = 2 });

            var isUnique = await _contentRepository
                .Users()
                .IsUsernameUnique(new IsUsernameUniqueQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Username = "test@cofoundry.org"
                })
                .ExecuteAsync();

            int userId;

            using (var scope = _contentRepository
                .Transactions()
                .CreateScope())
            {
                var adminRole = await _contentRepository
                    .Roles()
                    .GetByCode(SuperAdminRole.SuperAdminRoleCode)
                    .AsDetails()
                    .ExecuteAsync();

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
                    .AsSummary()
                    .ExecuteAsync();

                await _contentRepository
                    .CustomEntities()
                    .DataModelSchemas()
                    .GetByCustomEntityDefinitionCode(BlogPostCustomEntityDefinition.DefinitionCode)
                    .AsDetails()
                    .ExecuteAsync();

                await _contentRepository
                    .CustomEntities()
                    .GetById(1)
                    .AsRenderSummary()
                    .ExecuteAsync();

                var permissions = await _contentRepository
                    .Roles()
                    .Permissions()
                    .GetAll()
                    .AsIPermission()
                    .ExecuteAsync();

                var directoryTree = await _contentRepository
                    .PageDirectories()
                    .GetAll()
                    .AsTree()
                    .ExecuteAsync();

                var rewriteRules = await _contentRepository
                    .RewriteRules()
                    .GetAll()
                    .AsSummaries()
                    .ExecuteAsync();

                var blockTypes = await _contentRepository
                    .PageBlockTypes()
                    .GetAll()
                    .AsSummaries()
                    .ExecuteAsync();

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
