using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Parameters for mapping an IErrorPageViewModel using an
    /// IPageViewModelBuilder implementation.
    /// </summary>
    /// <remarks>
    /// It's possible for the mapper to be overriden and customized so 
    /// this parameter class helps future proof the design in case we 
    /// want to add more parameters in the future.
    /// </remarks>
    public class ErrorPageViewModelBuilderParameters
    {
        /// <summary>
        /// Http status code that represents the error e.g. 500, 404, 403.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Base path i.e. virtual directory of the requested path.
        /// </summary>
        public string PathBase { get; set; }

        /// <summary>
        /// The path of the page requested that could not be found.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Full querystring of the requested path with leading ? character.
        /// </summary>
        public string QueryString { get; set; }
    }
}