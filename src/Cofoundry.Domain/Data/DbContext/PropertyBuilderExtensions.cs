using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Data
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<TProperty> IsCharType<TProperty>(this PropertyBuilder<TProperty> builder, int length)
        {
            builder
                .HasColumnType($"char({ length })")
                .HasMaxLength(length)
                .IsUnicode(false);

            return builder;
        }

        public static PropertyBuilder<TProperty> IsNVarCharMaxType<TProperty>(this PropertyBuilder<TProperty> builder)
        {
            builder
                .HasColumnType("nvarchar(max)")
                .IsUnicode();

            return builder;
        }

        public static PropertyBuilder<TProperty> IsVarCharMaxType<TProperty>(this PropertyBuilder<TProperty> builder)
        {
            builder
                .HasColumnType("varchar(max)")
                .IsUnicode(false);

            return builder;
        }
    }
}
