using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Simple builder class for creating query strings in a neat and concise way.
    /// </summary>
    public class QueryStringBuilder
    {
        private List<Tuple<string, string>> _values = new List<Tuple<string, string>>();

        #region statics

        /// <summary>
        /// Shorthand for creating a querystring from just one parameter
        /// </summary>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">value  of the parameter</param>
        /// <returns>The full querystring including the leading ?</returns>
        public static string Create(string key, string value)
        {
            var builder = new QueryStringBuilder();
            builder.Add(key, value);

            return builder.Render();
        }

        /// <summary>
        /// Shorthand for creating a querystring from just one parameter
        /// </summary>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">value  of the parameter</param>
        /// <returns>The full querystring including the leading ?</returns>
        public static string Create<T>(string key, T value)
        {
            return Create(key, Convert.ToString(value));
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds a new parameter to the queystring if the value is not null or 
        /// empty.
        /// </summary>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">value  of the parameter</param>
        /// <returns>QueryStringBuilder instance for method chaining.</returns>
        public QueryStringBuilder Add(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentEmptyException(nameof(key));

            if (!string.IsNullOrWhiteSpace(value))
            {
                _values.Add(new Tuple<string, string>(key, value));
            }

            return this;
        }

        /// <summary>
        /// Adds a new parameter to the queystring if the value is not null.
        /// </summary>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">value  of the parameter</param>
        /// <returns>QueryStringBuilder instance for method chaining.</returns>
        public QueryStringBuilder Add<T>(string key, T value)
        {
            return Add(key, Convert.ToString(value));
        }

        /// <summary>
        /// Renders the state of the builder to a querystring including the leading question
        /// mark. If there are no parameters then an empty string is returned.
        /// </summary>
        /// <returns>The rendered querystring.</returns>
        public string Render()
        {
            return Render(_values);
        }

        /// <summary>
        /// Renders the state of the builder to a querystring including the leading question
        /// mark. Items are rendered in alphabetical order to ensure consistency. If there 
        /// are no parameters then an empty string is returned.
        /// </summary>
        /// <returns>The rendered querystring.</returns>
        public string OrderAndRender()
        {
            if (!_values.Any())
            {
                return string.Empty;
            }

            return Render(_values
                .OrderBy(v => v.Item1)
                .ThenByDescending(v => v.Item2)
                );
        }

        private static string Render(IEnumerable<Tuple<string, string>> itemsToRender)
        {
            if (!itemsToRender.Any())
            {
                return string.Empty;
            }

            return "?" + string.Join("&", itemsToRender
                .Select(p => p.Item1 + "=" + Uri.EscapeDataString(p.Item2)));
        }

        #endregion
    }
}
