using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Inheirt from this to define a configuration source for an
    /// html editor (typically TinyMCE).
    /// </summary>
    public interface IHtmlEditorConfigSource
    {
        /// <summary>
        /// Creates a collection of TinyMCE configuration options that
        /// will be applied after the Cofoundry defaults.
        /// </summary>
        IDictionary<string, object> Create();
    }
}
