using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Default implementation of <see cref="IClaimsPrincipalBuilderContextRepository"/>.
/// </summary>
public class ClaimsPrincipalBuilderContextRepository : IClaimsPrincipalBuilderContextRepository
{
    private readonly CofoundryDbContext _cofoundryDbContext;

    public ClaimsPrincipalBuilderContextRepository(
        CofoundryDbContext cofoundryDbContext
        )
    {
        _cofoundryDbContext = cofoundryDbContext;
    }

    /// <inheritdoc/>
    public async Task<IClaimsPrincipalBuilderContext?> GetAsync(int userId)
    {
        if (userId < 1)
        {
            return null;
        }

        var result = await _cofoundryDbContext
            .Users
            .AsNoTracking()
            .FilterCanSignIn()
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
