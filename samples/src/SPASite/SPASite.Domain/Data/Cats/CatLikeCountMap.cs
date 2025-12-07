using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPASite.Data;

public class CatLikeCountMap : IEntityTypeConfiguration<CatLikeCount>
{
    public void Configure(EntityTypeBuilder<CatLikeCount> builder)
    {
        builder.ToTable(nameof(CatLikeCount), DbConstants.DefaultAppSchema);

        builder.HasKey(e => e.CatCustomEntityId);
    }
}