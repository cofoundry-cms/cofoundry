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
    public class CultureContextService : ICultureContextService
    {
        private readonly ICultureFactory _cultureFactory;

        public CultureContextService(
            ICultureFactory cultureFactory
            )
        {
            _cultureFactory = cultureFactory;
        }

        public CultureInfo GetCurrent()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        public void SetCurrent(string ietfLanguageTag)
        {
            var culture = _cultureFactory.Create(ietfLanguageTag);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
