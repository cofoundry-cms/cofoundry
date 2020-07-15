using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time
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

        public DateTime UtcNow()
        {
            var dateTimeNow = DateTime.UtcNow;
            if (!_dateModifier.HasValue) return dateTimeNow;

            var modifiedDate = dateTimeNow.Add(_dateModifier.Value);

            return modifiedDate;
        }

        public DateTimeOffset OffsetUtcNow()
        {
            var dateTimeNow = DateTimeOffset.UtcNow;
            if (!_dateModifier.HasValue) return dateTimeNow;

            var modifiedDate = dateTimeNow.Add(_dateModifier.Value);

            return modifiedDate;
        }
    }
}
