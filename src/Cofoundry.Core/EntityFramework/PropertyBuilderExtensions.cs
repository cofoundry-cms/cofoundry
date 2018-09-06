using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

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
    }
}
