using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Defines a rule which can be used to exclude requests from
    /// having the visual editor attached to them. Multiple rules can 
    /// be defined, and each one is checked in turn to determine if a 
    /// request should be excluded.
    /// </summary>
    public interface IVisualEditorRequestExclusionRule
    {
        /// <summary>
        /// Returns true if the request should not have the
        /// visual editor code injected into it.
        /// </summary>
        /// <param name="request">Request to check.</param>
        bool ShouldExclude(HttpRequest request);
    }
}
