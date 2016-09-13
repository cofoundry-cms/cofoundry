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
    public interface ICultureFactory
    {
        CultureInfo Create(string languageTag);
    }
}
