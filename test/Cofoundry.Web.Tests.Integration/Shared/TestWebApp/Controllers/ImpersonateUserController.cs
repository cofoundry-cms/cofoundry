using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Tests.Integration.TestWebApp
{
    [ApiController]
    [Route("tests/impersonate-user")]
    public class ImpersonateUserController : ControllerBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly CofoundryDbContext _cofoundryDbContext;

        public ImpersonateUserController(
            IUserSessionService userSessionService,
            CofoundryDbContext cofoundryDbContext
            )
        {
            _userSessionService = userSessionService;
            _cofoundryDbContext = cofoundryDbContext;
        }

        [HttpPost("{id:int}")]
        public async Task LogIn(int id)
        {
            var userAreaCode = await _cofoundryDbContext
                .Users
                .AsNoTracking()
                .FilterById(id)
                .Select(u => u.UserAreaCode)
                .SingleOrDefaultAsync();

            if (userAreaCode == null) throw new Exception("User not found");

            await  _userSessionService.LogUserInAsync(userAreaCode, id, true);
        }
    }
}
