using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{

    /// <summary>
    /// A custom culter for English (United Arab Emirates) based on the en-GB culture.
    /// </summary>
    public class EnglishUAECulture : CultureInfo
    {
        public EnglishUAECulture()
            : base("en-GB")
        { }

        public override string ToString()
        {
            return "en-AE";
        }

        public override int LCID
        {
            get
            {
                return 999;
            }
        }

        public override string EnglishName
        {
            get
            {
                return "English - United Arab Emirates";
            }
        }

        public override string NativeName
        {
            get
            {
                return "English - United Arab Emirates";
            }
        }

        public override string DisplayName
        {
            get
            {
                return "English - United Arab Emirates";
            }
        }

        public override string Name
        {
            get
            {
                return "en-AE";
            }
        }
    }
}
