using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Parameters for mapping an INotFoundPageViewModel using an
    /// IPageViewModelBuilder implementation.
    /// </summary>
    /// <remarks>
    /// It's possible for the mapper to be overriden and customized so 
    /// this parameter class helps future proof the design in case we 
    /// want to add more parameters in the future.
    /// </remarks>
    public class NotFoundPageViewModelBuilderParameters
    {
        /// <summary>
        /// Parameters for mapping an INotFoundPageViewModel using an
        /// IPageViewModelMapper implementation.
        /// </summary>
        /// <param name="path">The path of the page requested that could not be found.</param>
        public NotFoundPageViewModelBuilderParameters(
               string path
               )
        {
            Path = path;
        }

        /// <summary>
        /// The path of the page requested that could not be found.
        /// </summary>
        public string Path { get; set; }
    }
}