using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class RewriteRuleMap : EntityTypeConfiguration<RewriteRule>
    {
        public RewriteRuleMap()
        {
            // Properties
            Property(t => t.WriteFrom)
                .IsRequired()
                .HasMaxLength(2000);
            Property(t => t.WriteTo)
                .IsRequired()
                .HasMaxLength(2000);
            
            CreateAuditableMappingHelper.Map(this);
        }
    }
}
