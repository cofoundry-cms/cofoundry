using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Locale
    {
        public Locale()
        {
            ChildLocales = new List<Locale>();
        }

        public int LocaleId { get; set; }
        public Nullable<int> ParentLocaleId { get; set; }
        public string IETFLanguageTag { get; set; }
        public string LocaleName { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Locale> ChildLocales { get; set; }
        public virtual Locale ParentLocale { get; set; }
    }
}
