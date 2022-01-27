using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Core.EntityFramework
{
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Defines the property as an MS SqlServer fixed length char type.
        /// </summary>
        /// <typeparam name="TProperty">Type of property being configured.</typeparam>
        /// <param name="builder">The property builder instance to act on and chain.</param>
        /// <param name="length">The fixed length of the char field.</param>
        /// <returns>Property builder for chaining.</returns>
        public static PropertyBuilder<TProperty> IsCharType<TProperty>(this PropertyBuilder<TProperty> builder, int length)
        {
            builder
                .HasColumnType($"char({ length })")
                .HasMaxLength(length)
                .IsUnicode(false);

            return builder;
        }

        /// <summary>
        /// Defines the property as an MS SqlServer nvarchar(max) field.
        /// </summary>
        /// <typeparam name="TProperty">Type of property being configured.</typeparam>
        /// <param name="builder">The property builder instance to act on and chain.</param>
        /// <returns>Property builder for chaining.</returns>
        public static PropertyBuilder<TProperty> IsNVarCharMaxType<TProperty>(this PropertyBuilder<TProperty> builder)
        {
            builder
                .HasColumnType("nvarchar(max)")
                .IsUnicode();

            return builder;
        }

        /// <summary>
        /// Defines the property as an MS SqlServer varchar(max) field.
        /// </summary>
        /// <typeparam name="TProperty">Type of property being configured.</typeparam>
        /// <param name="builder">The property builder instance to act on and chain.</param>
        /// <returns>Property builder for chaining.</returns>
        public static PropertyBuilder<TProperty> IsVarCharMaxType<TProperty>(this PropertyBuilder<TProperty> builder)
        {
            builder
                .HasColumnType("varchar(max)")
                .IsUnicode(false);

            return builder;
        }

        /// <summary>
        /// Indicates that the field is a UTC <see cref="DateTime"/>, ensuring it is specified as
        /// <see cref="DateTimeKind.Utc"/> when it is mapped from the database.
        /// </summary>
        public static PropertyBuilder<DateTime> IsUtc(this PropertyBuilder<DateTime> propertyBuilder)
        {
            return propertyBuilder.HasConversion(i => i, o => DateTime.SpecifyKind(o, DateTimeKind.Utc));
        }

        /// <summary>
        /// Indicates that the field is a UTC <see cref="DateTime"/>, ensuring it is specified as
        /// <see cref="DateTimeKind.Utc"/> when it is mapped from the database.
        /// </summary>
        public static PropertyBuilder<DateTime?> IsUtc(this PropertyBuilder<DateTime?> propertyBuilder)
        {
            return propertyBuilder.HasConversion(i => i, o => o.HasValue ? DateTime.SpecifyKind(o.Value, DateTimeKind.Utc) : o);
        }
    }
}