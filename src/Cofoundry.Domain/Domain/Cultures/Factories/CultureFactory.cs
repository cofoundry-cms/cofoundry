using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A culture factory that allows the creation and registration of custom cultures.
    /// </summary>
    public class CultureFactory : ICultureFactory
    {
        public CultureInfo Create(string languageTag)
        {
            if (string.IsNullOrEmpty(languageTag))
            {
                throw new ArgumentException("Cannot create a culture with an empty language tag.", languageTag);
            }

            return new CultureInfo(languageTag);
        }
    }
}
