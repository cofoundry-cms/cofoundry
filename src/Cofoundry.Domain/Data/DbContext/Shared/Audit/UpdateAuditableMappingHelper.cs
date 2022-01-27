using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public static class UpdateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeBuilder<T> builder) where T : class, IUpdateAuditable
        {
            CreateAuditableMappingHelper.Map(builder);

            builder.HasOne(s => s.Updater)
                .WithMany()
                .HasForeignKey(d => d.UpdaterId);

            builder.Property(s => s.UpdateDate).IsUtc();
        }
    }
}
