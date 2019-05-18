using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    public class TestController : Controller
    {
        private readonly IContentRepository _contentRepository;

        public TestController(
            IContentRepository contentRepository
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

            var isUnique = await _contentRepository
                .Users()
                .IsUsernameUniqueAsync(new IsUsernameUniqueQuery()
                {
                    UserAreaCode = CofoundryAdminUserArea.AreaCode,
                    Username = "test@cofoundry.org"
                });

            using (var scope = _contentRepository.Transactions().Create())
            {
                await _contentRepository
                    .WithElevatedPermissions()
                    .Users()
                    .AddAsync(new AddUserCommand()
                    {
                        Email = "test@cofoundry.org",
                        Password = "badpassword"
                    });

                await _contentRepository
                    .WithElevatedPermissions()
                    .ImageAssets()
                    .DeleteAsync(2);



                await scope.CompleteAsync();
            }


            return View();
        }
    }
}
