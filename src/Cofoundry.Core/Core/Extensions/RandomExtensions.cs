using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Not just random stuff but extension methods on instances of the Random class. 
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Gets a random decimal number.
        /// </summary>
        /// <param name="rnd">The instance to run the command on.</param>
        /// <param name="minValue">The inclusive minimum value to be returned. Defaults to 0.</param>
        /// <param name="maxValue">The exclusive maximum value to be returned. Defaults to 1</param>
        /// <returns>A random decimal value.</returns>
        public static decimal NextDecimal(this Random rnd, decimal? minValue = null, decimal? maxValue = null)
        {
            if (rnd == null) throw new ArgumentNullException(nameof(rnd));

            var max = Convert.ToInt32(Math.Floor(maxValue ?? minValue ?? 0));
            var min = Convert.ToInt32(Math.Ceiling(minValue ?? 1));

            if (min > max)
            {
                throw new ArgumentException($"{nameof(minValue)} cannot be greater than {nameof(maxValue)}", nameof(minValue));
            }

            var i = rnd.Next(min, max);
            var exp = rnd.NextDouble();
            var dec = Convert.ToDecimal(i + exp);
            return dec;
        }
    }
}
