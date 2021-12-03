using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class EmailDomainMap : IEntityTypeConfiguration<EmailDomain>
    {
        public void Configure(EntityTypeBuilder<EmailDomain> builder)
        {
            builder.ToTable("EmailDomain", DbConstants.CofoundrySchema);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
