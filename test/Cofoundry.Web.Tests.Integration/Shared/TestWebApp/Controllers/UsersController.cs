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
    [Route("tests/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly CofoundryDbContext _cofoundryDbContext;
        private readonly IAdvancedContentRepository _contentRepository;

        public UsersController(
            IUserSessionService userSessionService,
            CofoundryDbContext cofoundryDbContext,
            IAdvancedContentRepository contentRepository
            )
        {
            _userSessionService = userSessionService;
            _cofoundryDbContext = cofoundryDbContext;
            _contentRepository = contentRepository;
        }

        [HttpPost("impersonate/{id:int}")]
        public async Task Impersonate(int id)
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

        [HttpGet("current")]
        public async Task<int?> Current(string userAreaCode)
        {
            int? userId;

            if (string.IsNullOrEmpty(userAreaCode))
            {
                userId = _userSessionService.GetCurrentUserId();
            }
            else
            {
                userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
            }

            return userId;
        }

        /// <summary>
        /// Used by ClaimsPrincipalValidatorTests
        /// </summary>
        [HttpPut("password/{id:int}")]
        public async Task UpdatePassword(int id)
        {
            await _contentRepository
                .ExecuteCommandAsync(new UpdateUserPasswordByUserIdCommand()
                {
                    NewPassword = "UpdatedPassword",
                    UserId = id
                });
        }
    }
}
