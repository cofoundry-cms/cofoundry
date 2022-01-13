using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Internal
{
    /// <inheritdoc/>
    public class ClaimsPrincipalBuilderContextRepository : IClaimsPrincipalBuilderContextRepository
    {
        private readonly CofoundryDbContext _cofoundryDbContext;

        public ClaimsPrincipalBuilderContextRepository(
            CofoundryDbContext cofoundryDbContext
            )
        {
            _cofoundryDbContext = cofoundryDbContext;
        }

        public async Task<IClaimsPrincipalBuilderContext> GetAsync(int userId)
        {
            if (userId < 1) return null;

            var result = await _cofoundryDbContext
                .Users
                .AsNoTracking()
                .FilterCanLogIn()
                .FilterById(userId)
                .Select(u => new ClaimsPrincipalBuilderContext()
                {
                    SecurityStamp = u.SecurityStamp,
                    UserAreaCode = u.UserAreaCode,
                    UserId = u.UserId
                })
                .SingleOrDefaultAsync();

            return result;
        }
    }
}
