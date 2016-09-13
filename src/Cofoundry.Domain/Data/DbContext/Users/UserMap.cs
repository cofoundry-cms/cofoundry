using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("User", DbConstants.CofoundrySchema);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(32);

            Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(32);

            Property(t => t.Email)
                .HasMaxLength(150);

            Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(150);

            Property(t => t.Password)
                .IsMaxLength();

            Property(t => t.UserAreaCode)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false);

            // Relationships
            HasRequired(t => t.Role)
                .WithMany()
                .HasForeignKey(d => d.RoleId);

            #region create auditable (ish)

            // Relationships
            HasOptional(t => t.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatorId);

            HasRequired(s => s.UserArea)
                .WithMany(d => d.Users)
                .HasForeignKey(d => d.UserAreaCode);

            #endregion
        }
    }
}
