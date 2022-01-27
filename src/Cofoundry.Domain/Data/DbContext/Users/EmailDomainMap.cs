using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class EmailDomainMap : IEntityTypeConfiguration<EmailDomain>
    {
        public void Configure(EntityTypeBuilder<EmailDomain> builder)
        {
            builder.ToTable(nameof(EmailDomain), DbConstants.CofoundrySchema);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.CreateDate).IsUtc();
        }
    }
}