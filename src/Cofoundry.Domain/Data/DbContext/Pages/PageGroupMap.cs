using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroupMap : IEntityTypeConfiguration<PageGroup>
{
    public void Configure(EntityTypeBuilder<PageGroup> builder)
    {
        builder.ToTable(nameof(PageGroup), DbConstants.CofoundrySchema);

        builder.Property(s => s.GroupName)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(s => s.ParentPageGroup)
            .WithMany(s => s.ChildPageGroups)
            .HasForeignKey(d => d.ParentGroupId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
