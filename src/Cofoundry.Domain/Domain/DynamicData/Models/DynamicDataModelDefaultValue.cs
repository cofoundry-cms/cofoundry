using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object for a default dynamic data model instance.
    /// The wrapper is required to allow a custom JSON converter to
    /// run on the instance, which removed default property values.
    /// </summary>
    public class DynamicDataModelDefaultValue
    {
        /// <summary>
        /// A new instance of the model with any default values.
        /// </summary>
        public object Value { get; set; }
    }
}
