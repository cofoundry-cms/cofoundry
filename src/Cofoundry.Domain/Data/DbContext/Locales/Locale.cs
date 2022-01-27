using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class Locale
    {
        public int LocaleId { get; set; }
        public Nullable<int> ParentLocaleId { get; set; }
        public string IETFLanguageTag { get; set; }
        public string LocaleName { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Locale> ChildLocales { get; set; } = new List<Locale>();
        public virtual Locale ParentLocale { get; set; }
    }
}