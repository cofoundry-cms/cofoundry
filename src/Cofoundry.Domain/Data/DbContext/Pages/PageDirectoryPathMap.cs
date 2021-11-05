using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageDirectoryPathMap : IEntityTypeConfiguration<PageDirectoryPath>
    {
        public void Configure(EntityTypeBuilder<PageDirectoryPath> builder)
        {
            builder.ToTable("PageDirectoryPath", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.PageDirectoryId);

            // Properties
            builder.Property(s => s.FullUrlPath)
                .IsRequired()
                .IsNVarCharMaxType();
        }
    }
}
