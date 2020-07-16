using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time.Mocks
{
    /// <summary>
    /// IDateTimeService implementation with a detemanistic time value
    /// that can be altered as needed.
    /// </summary>
    public class MockDateTimeService : IDateTimeService
    {
        public MockDateTimeService() { }
        public MockDateTimeService(DateTimeOffset mockDateTime)
        {
            MockDateTime = mockDateTime;
        }
        public DateTimeOffset? MockDateTime { get; set; }

        public DateTimeOffset OffsetUtcNow()
        {
            return MockDateTime ?? DateTimeOffset.UtcNow;
        }

        public DateTime UtcNow()
        {
            return OffsetUtcNow().UtcDateTime;
        }
    }
}
