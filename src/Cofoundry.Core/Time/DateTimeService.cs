using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time.Internal
{
    public class DateTimeService : IDateTimeService
    {
        private TimeSpan? _dateModifier;

        public DateTimeService(
            DateTimeSettings dateTimeSettings
            )
        {
            if (!dateTimeSettings.BaseDate.HasValue) return;

            var dateTimeNow = DateTime.UtcNow;

            _dateModifier = dateTimeSettings.BaseDate - dateTimeNow;
        }

        public virtual DateTime UtcNow()
        {
            var dateTimeNow = DateTime.UtcNow;
            if (!_dateModifier.HasValue) return dateTimeNow;

            var modifiedDate = dateTimeNow.Add(_dateModifier.Value);

            return modifiedDate;
        }

        public virtual DateTimeOffset OffsetUtcNow()
        {
            var dateTimeNow = DateTimeOffset.UtcNow;
            if (!_dateModifier.HasValue) return dateTimeNow;

            var modifiedDate = dateTimeNow.Add(_dateModifier.Value);

            return modifiedDate;
        }
    }
}
