using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageDirectoryMap : IEntityTypeConfiguration<PageDirectory>
    {
        public void Configure(EntityTypeBuilder<PageDirectory> builder)
        {
            builder.ToTable("PageDirectory", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.PageDirectoryId);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            builder.HasOne(s => s.ParentPageDirectory)
                .WithMany(d => d.ChildPageDirectories)
                .HasForeignKey(s => s.ParentPageDirectoryId);

            builder.HasOne(s => s.UserAreaForLoginRedirect)
                .WithMany()
                .HasForeignKey(s => s.UserAreaCodeForLoginRedirect);

            // Relationships
            builder.HasOne(s => s.PageDirectoryPath)
                .WithOne(d => d.PageDirectory)
                .HasForeignKey<PageDirectoryPath>(s => s.PageDirectoryId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
