using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public static class CreateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeBuilder<T> builder) where T : class, ICreateAuditable
        {
            builder.Property(s => s.CreateDate).IsUtc();

            builder.HasOne(s => s.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatorId);
        }
    }
}
