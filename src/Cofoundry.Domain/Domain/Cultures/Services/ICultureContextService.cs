using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service abstraction over the culture of the current request.
    /// </summary>
    public interface ICultureContextService
    {
        CultureInfo GetCurrent();
        void SetCurrent(string ietfLanguageTag);
    }
}
