using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this interface to define the source for a list
    /// component in the admin UI that uses a simple api call
    /// to provide the data for its option collection.
    /// </summary>
    public interface IListOptionApiSource
    {
        /// <summary>
        /// The path the to the list option API. This should be relative
        /// e.g. /admin/api/example-options and conform to the expected 
        /// api response format used by IApiResponseHelper.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The field in the response object to display in the option.
        /// </summary>
        string NameField { get; }

        /// <summary>
        /// The field in the response object to use as the option value.
        /// </summary>
        string ValueField { get; }
    }
}
