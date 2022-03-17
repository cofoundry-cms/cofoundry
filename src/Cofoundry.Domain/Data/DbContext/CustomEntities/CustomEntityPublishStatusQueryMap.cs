using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class CustomEntityPublishStatusQueryMap : IEntityTypeConfiguration<CustomEntityPublishStatusQuery>
{
    public void Configure(EntityTypeBuilder<CustomEntityPublishStatusQuery> builder)
    {
        builder.ToTable(nameof(CustomEntityPublishStatusQuery), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.CustomEntityId, s.PublishStatusQueryId });

        builder.HasOne(s => s.CustomEntity)
            .WithMany(s => s.CustomEntityPublishStatusQueries)
            .HasForeignKey(d => d.CustomEntityId);

        builder.HasOne(s => s.CustomEntityVersion)
            .WithMany(s => s.CustomEntityPublishStatusQueries)
            .HasForeignKey(d => d.CustomEntityVersionId);
    }
}
