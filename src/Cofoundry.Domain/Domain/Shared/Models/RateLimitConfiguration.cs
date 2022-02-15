using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents configuration information for a rate limiter
    /// </summary>
    public class RateLimitConfiguration
    {
        public RateLimitConfiguration()
        {
        }

        /// <param name="quantity">
        /// The maximum number of occurances to allow within the
        /// time-window described by the <see cref="Window"/>
        /// property. If zero or less, then rate limiting does not occur.
        /// </param>
        /// <param name="window">
        /// The time window in which to count when rate limiting. If zero 
        /// or less, then rate limiting does not occur.
        /// </param>
        public RateLimitConfiguration(int quantity, TimeSpan window)
        {
            Quantity = quantity;
            Window = window;
        }

        /// <summary>
        /// The maximum number of occurances to allow within the
        /// time-window described by the <see cref="Window"/>
        /// property. If zero or less, then rate limiting does not occur.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The time window in which to count when rate limiting, specified as a 
        /// <see cref="TimeSpan"/> or in JSON configuration as a time format string 
        /// e.g. "00:30:00" to represent 30 minutes. If zero or less, then rate 
        /// limiting does not occur.
        /// </summary>
        public TimeSpan Window { get; set; }

        /// <summary>
        /// Determines if the quantity configuration is valid, checking that
        /// the <see cref="Quantity"/> is a positive value. Some rate limiting
        /// allows a quantity without a time-window.
        /// </summary>
        public bool HasValidQuantity()
        {
            return Quantity > 0;
        }

        /// <summary>
        /// Determines if the window configuration is valid, checking that
        /// the <see cref="Window"/> is a positive value.
        /// </summary>
        public bool HasValidWindow()
        {
            return Window > TimeSpan.Zero;
        }

        /// <summary>
        /// Determines if the configuration is valid, checking that
        /// the <see cref="Quantity"/> and <see cref="Window"/> are
        /// positive values.
        /// </summary>
        public bool HasValidQuantityAndWindow()
        {
            return Quantity > 0 && Window > TimeSpan.Zero;
        }

        /// <summary>
        /// Copies the rate limit data to a new instance.
        /// </summary>
        /// <returns></returns>
        public RateLimitConfiguration Clone()
        {
            return new RateLimitConfiguration()
            {
                Quantity = Quantity,
                Window = Window
            };
        }
    }
}