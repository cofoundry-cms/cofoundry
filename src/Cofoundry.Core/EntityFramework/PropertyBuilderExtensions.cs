using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Core.EntityFramework;

/// <summary>
/// Extension methods for <see cref="PropertyBuilder{T}"/>.
/// </summary>
public static class PropertyBuilderExtensions
{
    extension<TProperty>(PropertyBuilder<TProperty> builder)
    {
        /// <summary>
        /// Defines the property as an MS SqlServer fixed length char type.
        /// </summary>
        /// <param name="length">The fixed length of the char field.</param>
        /// <returns>Property builder for chaining.</returns>
        public PropertyBuilder<TProperty> IsCharType(int length)
        {
            builder
                .HasColumnType($"char({length})")
                .HasMaxLength(length)
                .IsUnicode(false);

            return builder;
        }

        /// <summary>
        /// Defines the property as an MS SqlServer nvarchar(max) field.
        /// </summary>
        /// <returns>Property builder for chaining.</returns>
        public PropertyBuilder<TProperty> IsNVarCharMaxType()
        {
            builder
                .HasColumnType("nvarchar(max)")
                .IsUnicode();

            return builder;
        }

        /// <summary>
        /// Defines the property as an MS SqlServer varchar(max) field.
        /// </summary>
        /// <returns>Property builder for chaining.</returns>
        public PropertyBuilder<TProperty> IsVarCharMaxType()
        {
            builder
                .HasColumnType("varchar(max)")
                .IsUnicode(false);

            return builder;
        }
    }

    extension(PropertyBuilder<DateTime> propertyBuilder)
    {
        /// <summary>
        /// Indicates that the field is a UTC <see cref="DateTime"/>, ensuring it is specified as
        /// <see cref="DateTimeKind.Utc"/> when it is mapped from the database.
        /// </summary>
        public PropertyBuilder<DateTime> IsUtc()
        {
            return propertyBuilder.HasConversion(i => i, o => DateTime.SpecifyKind(o, DateTimeKind.Utc));
        }
    }

    extension(PropertyBuilder<DateTime?> propertyBuilder)
    {
        /// <summary>
        /// Indicates that the field is a UTC <see cref="DateTime"/>, ensuring it is specified as
        /// <see cref="DateTimeKind.Utc"/> when it is mapped from the database.
        /// </summary>
        public PropertyBuilder<DateTime?> IsUtc()
        {
            return propertyBuilder.HasConversion(i => i, o => o.HasValue ? DateTime.SpecifyKind(o.Value, DateTimeKind.Utc) : o);
        }
    }
}
