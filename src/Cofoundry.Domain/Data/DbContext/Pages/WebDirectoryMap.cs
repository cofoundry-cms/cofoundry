using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class WebDirectoryMap : EntityTypeConfiguration<WebDirectory>
    {
        public WebDirectoryMap()
        {
            // Primary Key
            HasKey(t => t.WebDirectoryId);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(64);

            Property(t => t.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            HasOptional(t => t.ParentWebDirectory)
                .WithMany(t => t.ChildWebDirectories)
                .HasForeignKey(d => d.ParentWebDirectoryId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
