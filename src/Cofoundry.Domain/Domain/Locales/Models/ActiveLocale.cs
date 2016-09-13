using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A locale supported by the site. Although there is an integer primary key the unique idetifier for a locale is an 
    /// IETF language tag making a locale directly related to a language choice.
    /// </summary>
    public class ActiveLocale : IEquatable<ActiveLocale>
    {
        public int LocaleId { get; set; }

        /// <summary>
        /// An IETF language tag is an abbreviated language code (for example, en for English, pt-BR for 
        /// Brazilian Portuguese, or nan-Hant-TW for Min Nan Chinese as spoken in Taiwan using traditional 
        /// Han characters) defined by the Internet Engineering Task Force (IETF).
        /// </summary>
        public string IETFLanguageTag { get; set; }

        /// <summary>
        /// English name of the locale e.g. 'Spanish – Bolivia', 'English', 'English – Canada'
        /// </summary>
        public string Name { get; set; }

        #region methods

        #region IEquatable

        public bool Equals(ActiveLocale other)
        {
            if (other == null) return false;
            if (other == this) return true;

            return other.LocaleId == LocaleId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ActiveLocale);
        }

        public override int GetHashCode()
        {
            return LocaleId;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, IETFLanguageTag);
        }

        #endregion
    }
}
