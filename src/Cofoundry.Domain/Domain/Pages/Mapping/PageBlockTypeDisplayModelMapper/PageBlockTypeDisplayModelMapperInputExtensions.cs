using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// These extension methods help cut down on boilerplate code in model mappers.
    /// </summary>
    public static class PageBlockTypeDisplayModelMapperInputExtensions
    {
        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <typeparam name="TProperty">Selected property type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<TProperty> SelectDistinctModelValues<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, TProperty> selector
            )
            where TDataModel : IPageBlockTypeDataModel
        {
            var results = input
                .Select(i => i.DataModel)
                .Select(selector)
                .Distinct();

            return results;
        }

        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model, removing any null, empty or whitespace only strings from
        /// the results.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<string> SelectDistinctModelValuesWithoutEmpty<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, string> selector
            )
            where TDataModel : IPageBlockTypeDataModel
        {
            var results = input
                .SelectDistinctModelValues(selector)
                .Where(v => !string.IsNullOrWhiteSpace(v));

            return results;
        }

        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model, removing any default(TProperty) values from
        /// the results, e.g. for numeric types this will remove 0.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <typeparam name="TProperty">Selected property type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<TProperty> SelectDistinctModelValuesWithoutEmpty<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, TProperty> selector
            )
            where TDataModel : IPageBlockTypeDataModel
            where TProperty : struct
        {
            var results = input
                .SelectDistinctModelValues(selector)
                .Where(v => !v.Equals(default(TProperty)));

            return results;
        }

        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model, removing any null or default(TProperty) values from
        /// the results, e.g. for nullable numeric types this will remove null and 0.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <typeparam name="TProperty">Selected property type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<TProperty> SelectDistinctModelValuesWithoutEmpty<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, TProperty?> selector
            )
            where TDataModel : IPageBlockTypeDataModel
            where TProperty : struct
        {
            var results = input
                .SelectDistinctModelValues(selector)
                .Where(v => v.HasValue && !v.Value.Equals(default(TProperty)))
                .Select(v => v.Value);

            return results;
        }

        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model, removing any null values from the results.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <typeparam name="TProperty">Selected property type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<TProperty> GetDistinctModelValuesWithoutNulls<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, TProperty> selector
            )
            where TDataModel : IPageBlockTypeDataModel
            where TProperty : class
        {
            var results = input
                .SelectDistinctModelValues(selector)
                .Where(v => v != null);

            return results;
        }

        /// <summary>
        /// Filters all inputs returning distinct values of the selected property
        /// on the data model.
        /// </summary>
        /// <typeparam name="TDataModel">DataModel type.</typeparam>
        /// <typeparam name="TProperty">Selected property type.</typeparam>
        /// <param name="input">Mapper input to select data from.</param>
        /// <param name="selector">Selector for the property to extract values from.</param>
        /// <returns>IEnumerable containing the distinct (non duplicate) results of the selection.</returns>
        public static IEnumerable<TProperty> SelectManyDistinctModelValues<TDataModel, TProperty>(
            this IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> input,
            Func<TDataModel, IEnumerable<TProperty>> selector
            )
            where TDataModel : IPageBlockTypeDataModel
        {
            var results = input
                .Select(i => i.DataModel)
                .Where(i => selector(i) != null)
                .SelectMany(selector)
                .Distinct();

            return results;
        }
    }
}
