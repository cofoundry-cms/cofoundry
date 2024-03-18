using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageMap : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable(nameof(Page), DbConstants.CofoundrySchema, t =>
        {
            t.HasTrigger("Cofoundry.Page_CascadeDelete");
        });

        builder.Property(s => s.UrlPath)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.CustomEntityDefinitionCode)
            .HasMaxLength(6)
            .IsUnicode(false);

        builder.Property(s => s.PublishStatusCode)
            .HasMaxLength(1)
            .IsUnicode(false);

        builder.HasOne(s => s.Locale)
            .WithMany()
            .HasForeignKey(d => d.LocaleId);

        builder.HasOne(s => s.CustomEntityDefinition)
            .WithMany()
            .HasForeignKey(d => d.CustomEntityDefinitionCode);

        builder.HasOne(s => s.PageDirectory)
            .WithMany(s => s.Pages)
            .HasForeignKey(d => d.PageDirectoryId);

        builder.HasOne(s => s.UserAreaForSignInRedirect)
            .WithMany()
            .HasForeignKey(s => s.UserAreaCodeForSignInRedirect);

        builder.Property(s => s.PublishDate).IsUtc();
        builder.Property(s => s.LastPublishDate).IsUtc();

        CreateAuditableMappingHelper.Map(builder);
    }
}
