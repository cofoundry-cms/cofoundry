using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;

namespace Cofoundry.Web
{
    /// <summary>
    /// A simple container for wrapping data output via a rest api. Used
    /// to provide a consistent response from a rest api and to 
    /// </summary>
    /// <typeparam name="T">Type of the data being returned</typeparam>
    public class SimpleCommandResponseData<T> : SimpleCommandResponseData
    {
        /// <summary>
        /// Any additional data to send back to the response.
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// A simple data container for returning the result of a command from a 
    /// rest api in a consistent and structured way.
    /// </summary>
    public class SimpleCommandResponseData
    {
        /// <summary>
        /// True if the command executed successfully; otherwise false.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Collection of any validation errors discovered when executing the request
        /// </summary>
        public ICollection<ValidationError> Errors { get; set; }
    }
}
