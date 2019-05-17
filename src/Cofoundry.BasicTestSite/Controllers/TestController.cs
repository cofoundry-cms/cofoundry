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

        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            var user = await _contentRepository
                .WithElevatedPermissions()
                .Users()
                .GetById(2)
                .AsMicroSummary();

            var isUnique = await _contentRepository
                .Users()
                .IsUsernameUnique(new IsUsernameUniqueQuery()
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

                await scope.CompleteAsync();
            }


            return View();
        }
    }
}
