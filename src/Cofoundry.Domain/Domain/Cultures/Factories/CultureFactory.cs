using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A culture factory that allows the creation and registration of custom cultures.
    /// </summary>
    public class CultureFactory : ICultureFactory
    {
        public CultureInfo Create(string languageTag)
        {
            return new CultureInfo(languageTag);
        }
    }
}
