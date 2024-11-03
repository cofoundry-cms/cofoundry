using Cofoundry.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Plugins.ErrorLogging.Data;

public class ErrorMap : IEntityTypeConfiguration<Error>
{
    public void Configure(EntityTypeBuilder<Error> builder)
    {
        builder.ToTable("Error", DbConstants.CofoundryPluginSchema);

        // Properties
        builder.Property(t => t.ExceptionType)
            .IsRequired();

        builder.Property(t => t.Url)
            .HasMaxLength(255);

        builder.Property(t => t.Source)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Target)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.StackTrace)
            .IsRequired();

        builder.Property(t => t.QueryString)
            .HasMaxLength(255);

        builder.Property(t => t.UserAgent)
            .HasMaxLength(255);
    }
}
