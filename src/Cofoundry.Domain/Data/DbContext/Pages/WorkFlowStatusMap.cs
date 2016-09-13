using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class WorkFlowStatusMap : EntityTypeConfiguration<WorkFlowStatus>
    {
        public WorkFlowStatusMap()
        {
            // Primary Key
            HasKey(t => t.WorkFlowStatusId);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(32);
        }
    }
}
