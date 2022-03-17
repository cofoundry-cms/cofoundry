using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PagePublishStatusQueryMap : IEntityTypeConfiguration<PagePublishStatusQuery>
{
    public void Configure(EntityTypeBuilder<PagePublishStatusQuery> builder)
    {
        builder.ToTable(nameof(PagePublishStatusQuery), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.PageId, s.PublishStatusQueryId });

        builder.HasOne(s => s.Page)
            .WithMany(s => s.PagePublishStatusQueries)
            .HasForeignKey(d => d.PageId);

        builder.HasOne(s => s.PageVersion)
            .WithMany(s => s.PagePublishStatusQueries)
            .HasForeignKey(d => d.PageVersionId);
    }
}
