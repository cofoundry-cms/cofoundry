using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        /// <summary>
        /// Gets the CultureInfo used by the current request.
        /// </summary>
        public CultureInfo GetCurrent()
        {
            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Sets the current thread culture and UI culture.
        /// </summary>
        /// <param name="ietfLanguageTag">An IETF language tag to set the current thread culture to e.g. 'en-US' or 'es'.</param>
        public void SetCurrent(string ietfLanguageTag)
        {
            if (string.IsNullOrEmpty(ietfLanguageTag))
            {
                throw new ArgumentException("Cannot set the current culture to empty.", ietfLanguageTag);
            }

            var culture = _cultureFactory.Create(ietfLanguageTag);

            if (culture != null)
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }
    }
}
