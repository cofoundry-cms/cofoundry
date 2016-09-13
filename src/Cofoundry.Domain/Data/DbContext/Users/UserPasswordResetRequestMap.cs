using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class UserPasswordResetRequestMap : EntityTypeConfiguration<UserPasswordResetRequest>
    {
        public UserPasswordResetRequestMap()
        {
            ToTable("UserPasswordResetRequest", DbConstants.CofoundrySchema);

            Property(t => t.Token)
                .IsUnicode(false)
                .IsMaxLength();

            Property(t => t.IPAddress)
                .IsUnicode(false)
                .HasMaxLength(45);

            // Relationships
            HasRequired(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);
        }
    }
}
