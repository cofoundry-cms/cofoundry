using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// Use to allow a set of configuration settings to be toggled on/off
    /// using an 'Enabled' property. If disabled, the feature will not 
    /// trigger validation of settings properties.
    /// </summary>
    public interface IFeatureEnableable
    {
        /// <summary>
        /// True if the feature is enabled; otheriwse false. Disabled
        /// features do not trigger property validation.
        /// </summary>
        public bool Enabled { get; }
    }
}
